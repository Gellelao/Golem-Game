using GolemCore.Models.Effects;
using GolemCore.Models.Enums;
using GolemCore.Models.Golem;
using GolemCore.Models.Part;
using Serilog;

namespace GolemCore.Resolver;

public class CombatResolver
{
  private Golem _user;
  private Golem _opponent;
  private GolemStats _userStats;
  private GolemStats _opponentStats;
  private PartsCache _cache;
  private readonly ILogger _log;
  private List<string> _results;
  private Queue<List<StatChange>> _statChangeQueue;

  public CombatResolver(Golem user, Golem opponent, PartsCache partsCache)
  {
    _user = user;
    _opponent = opponent;
    _userStats = new GolemStats(user, partsCache);
    _opponentStats = new GolemStats(opponent, partsCache);
    _cache = partsCache;
    _results = new List<string>();

    _statChangeQueue = new Queue<List<StatChange>>();
    
    _log = new LoggerConfiguration()
      .MinimumLevel.Debug()
      .Enrich.WithProperty("Project", "GolemCore")
      .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Project} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
      .CreateLogger();
  }

  public List<string> GetOutcome()
  {
    var turnCounter = 0;

    LogAndAddToResult($"User stats: {_userStats}");
    LogAndAddToResult($"Opponent stats: {_opponentStats}");

    var momentumTracker = new MomentumTracker(_userStats, _opponentStats, _log);

    _log.Debug("Combat begin =====================");
    while (_userStats.Get(StatType.Health) > 0 && _opponentStats.Get(StatType.Health) > 0)
    {
      turnCounter++; // this makes the first turn "turn 1" instead of zero
      
      var nextToGo = momentumTracker.GetNextGolem();
      _log.Debug("Next to attack is {a} ", nextToGo);

      switch (nextToGo)
      {
        case AttackOrder.User:
          LogAndAddToResult($"You attack the opponent");
          _statChangeQueue.Enqueue(new List<StatChange>
            {
              new()
              {
                TargetsPlayerGolem = false,
                TargetsSelf = false,
                StatType = StatType.Health,
                Delta = _userStats.Get(StatType.Attack) * -1
              }
            });
          break;
        case AttackOrder.Opponent:
          LogAndAddToResult($"The opponent attacks you");
          _statChangeQueue.Enqueue(new List<StatChange>
          {
            new()
            {
              TargetsPlayerGolem = true,
              TargetsSelf = false,
              StatType = StatType.Health,
              Delta = _opponentStats.Get(StatType.Attack) * -1
            }
          });
          break;
        case AttackOrder.Simultaneous:
          LogAndAddToResult($"You attack each other at the same time");
          _statChangeQueue.Enqueue(new List<StatChange>
          {
            new()
            {
              TargetsPlayerGolem = false,
              TargetsSelf = false,
              StatType = StatType.Health,
              Delta = _userStats.Get(StatType.Attack) * -1
            },
            new()
            {
              TargetsPlayerGolem = true,
              TargetsSelf = false,
              StatType = StatType.Health,
              Delta = _opponentStats.Get(StatType.Attack) * -1
            }
          });
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      while (_statChangeQueue.Any())
      {
        var statChanges = _statChangeQueue.Dequeue();
        ApplyStatChanges(statChanges);
        _log.Debug("Applying statChange {@a}", statChanges);
        
        var usersActivatedParts = new List<Part>();
        var opponentsActivatedParts = new List<Part>();
        foreach (var statChange in statChanges)
        {
          usersActivatedParts.AddRange(_user.GetPartsActivatedByStatChange(statChange, _cache));
          opponentsActivatedParts.AddRange(_opponent.GetPartsActivatedByStatChange(statChange, _cache));
        }
        
        _log.Debug("Number of User parts that were activated: {a}, Number of Opponent parts that were activated: {b}", usersActivatedParts.Count, opponentsActivatedParts.Count);

        var statChangesFromUserEffects = GetStatChangesFromTriggeredPartEffects(usersActivatedParts, true);
        var statChangesFromOpponentEffects = GetStatChangesFromTriggeredPartEffects(opponentsActivatedParts, false);
        var combinedEffects = statChangesFromUserEffects.Concat(statChangesFromOpponentEffects).ToList();
        if (combinedEffects.Any())
        {
          _log.Debug("Queuing statChange {@a}", combinedEffects);
          _statChangeQueue.Enqueue(combinedEffects);
        }
      }

      var userTurnActivatedParts = GetStatChangesFromTriggeredPartEffects(_user.GetPartsActivatedByTurn(turnCounter, _cache), true);
      var opponentTurnActivatedParts = GetStatChangesFromTriggeredPartEffects(_opponent.GetPartsActivatedByTurn(turnCounter, _cache), false);
      var statChangesFromTurn = userTurnActivatedParts.Concat(opponentTurnActivatedParts).ToList();
      _log.Debug("Any stat changes from turn counter: {a}", statChangesFromTurn.Any());
      if (statChangesFromTurn.Any())
      {
        ApplyStatChanges(statChangesFromTurn);
      }

      LogAndAddToResult($"--------------turn {turnCounter} over");
      
      if (turnCounter >= Constants.TurnLimit)
      {
        LogAndAddToResult($"Turn limit has been reached after {turnCounter} turns. Game is a draw!");
        break;
      }
    }

    return _results;
  }

  private List<StatChange> GetStatChangesFromTriggeredPartEffects(List<Part> activatedParts, bool partsBelongToPlayersGolem)
  {
    var golemStats = partsBelongToPlayersGolem ? _userStats : _opponentStats;
    List<StatChange> changesFromEffects = new List<StatChange>();
    foreach (var part in activatedParts)
    {
      LogAndAddToResult($"{part.Name} activated!");
      foreach (var effect in part.Effects)
      {
        if (!effect.RequirementMet(_userStats, _opponentStats))
        {
          LogAndAddToResult($"{part.Name} effect requirement is not met");
          break;
        }

        if (!effect.HasChargesLeft(golemStats.GetEffectTriggerCount(effect)))
        {
          LogAndAddToResult($"{part.Name} effect has no charges remaining");
          break;
        }

        golemStats.IncrementEffectTriggerCount(effect);

        switch (effect)
        {
          case StatChangeEffect statChangeEffect:
            var targetsSelf = statChangeEffect.Target == Target.Self;
            var targetsPlayerGolem = (targetsSelf && partsBelongToPlayersGolem) || !targetsSelf && !partsBelongToPlayersGolem;
            changesFromEffects.Add(new StatChange
            {
              TargetsPlayerGolem = targetsPlayerGolem,
              TargetsSelf = targetsSelf,
              StatType = statChangeEffect.Stat,
              Delta = statChangeEffect.Delta
            });
            LogAndAddToResult($"Effect has changed {(targetsPlayerGolem ? "your" : "your opponent's")} {statChangeEffect.Stat} by {statChangeEffect.Delta}! Now at {(targetsPlayerGolem ? _userStats.Get(StatType.Health) : _opponentStats.Get(StatType.Health))}");
            break;
        }
      }
    }

    return changesFromEffects;
  }

  private void ApplyStatChanges(List<StatChange> statChanges)
  {
    foreach (var statChange in statChanges)
    {
      if (statChange.TargetsPlayerGolem)
      {
        LogAndAddToResult($"Your golem has its {statChange.StatType} changed by {statChange.Delta}");
        _userStats.Update(statChange.StatType, statChange.Delta);
      }
      else
      {
        LogAndAddToResult($"Your opponent has its {statChange.StatType} changed by {statChange.Delta}");
        _userStats.Update(statChange.StatType, statChange.Delta);
      }
    }
  }

  private void LogAndAddToResult(string message)
  {
    _log.Debug(message);
    _results.Add(message);
  }
}

