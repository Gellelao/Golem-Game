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
  private Queue<StatChange> _statChangeQueue;

  public CombatResolver(Golem user, Golem opponent, PartsCache partsCache)
  {
    _user = user;
    _opponent = opponent;
    _userStats = new GolemStats(user, partsCache);
    _opponentStats = new GolemStats(opponent, partsCache);
    _cache = partsCache;
    _results = new List<string>();

    _statChangeQueue = new Queue<StatChange>();
    
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
          _statChangeQueue.Enqueue(new StatChange
          {
            Changes = new List<(Target, StatType, int)>
            {
              (Target.Opponent, StatType.Health, _userStats.Get(StatType.Attack) * -1)
            }
          });
          break;
        case AttackOrder.Opponent:
          LogAndAddToResult($"The opponent attacks you");
          _statChangeQueue.Enqueue(new StatChange
          {
            Changes = new List<(Target, StatType, int)>
            {
              (Target.Self, StatType.Health, _opponentStats.Get(StatType.Attack) * -1)
            }
          });
          break;
        case AttackOrder.Simultaneous:
          LogAndAddToResult($"You attack each other at the same time");
          _statChangeQueue.Enqueue(new StatChange
          {
            Changes = new List<(Target, StatType, int)>
            {
              (Target.Self, StatType.Health, _opponentStats.Get(StatType.Attack) * -1),
              (Target.Opponent, StatType.Health, _userStats.Get(StatType.Attack) * -1)
            }
          });
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      while (_statChangeQueue.Any())
      {
        var statChange = _statChangeQueue.Dequeue();
        ApplyStatChanges(statChange);
        _log.Debug("Applying statChange {@a}", statChange.Changes);
        
        var userActivatedParts = new List<Part>();
        var opponentActivatedParts = new List<Part>();
        foreach (var (target, statType, delta) in statChange.Changes)
        {
          switch (target)
          {
            case Target.Self:
              userActivatedParts.AddRange(_user.GetPartsActivatedByStatChange(statChange, _cache));
              break;
            case Target.Opponent:
              opponentActivatedParts.AddRange(_opponent.GetPartsActivatedByStatChange(statChange, _cache));
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }
        }
        
        _log.Debug("Number of user activated parts:     {a}", userActivatedParts.Count);
        _log.Debug("Number of opponent activated parts: {a}", opponentActivatedParts.Count);

        var statChangesFromUserEffects = GetStatChangesFromTriggeredPartEffects(userActivatedParts, _userStats);
        var statChangesFromOpponentEffects = GetStatChangesFromTriggeredPartEffects(opponentActivatedParts, _opponentStats);
        var combinedEffects = statChangesFromUserEffects.Concat(statChangesFromOpponentEffects).ToList();
        if (combinedEffects.Any())
        {
          _log.Debug("Queuing statChange {@a}", combinedEffects);
          _statChangeQueue.Enqueue(new StatChange
          {
            Changes = combinedEffects
          });
        }
      }

      var userTurnActivatedParts = GetStatChangesFromTriggeredPartEffects(_user.GetPartsActivatedByTurn(turnCounter, _cache), _userStats);
      var opponentTurnActivatedParts = GetStatChangesFromTriggeredPartEffects(_opponent.GetPartsActivatedByTurn(turnCounter, _cache), _opponentStats);
      var statChangesFromTurn = userTurnActivatedParts.Concat(opponentTurnActivatedParts).ToList();
      _log.Debug("Any stat changes from turn counter: {a}", statChangesFromTurn.Any());
      if (statChangesFromTurn.Any())
      {
        ApplyStatChanges(new StatChange
        {
          Changes = statChangesFromTurn
        });
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

  private List<(Target, StatType, int)> GetStatChangesFromTriggeredPartEffects(List<Part> activatedParts, GolemStats golemStats)
  {
    List<(Target, StatType, int)> changesFromEffects = new List<(Target, StatType, int)>();
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
            changesFromEffects.Add((statChangeEffect.Target, statChangeEffect.Stat, statChangeEffect.Delta));
            LogAndAddToResult($"Effect has changed {statChangeEffect.Target} {statChangeEffect.Stat} by {statChangeEffect.Delta}! Now at {(statChangeEffect.Target == Target.Opponent ? _opponentStats.Get(StatType.Health) : _userStats.Get(StatType.Health))}");
            break;
        }
      }
    }

    return changesFromEffects;
  }

  private void ApplyStatChanges(StatChange statChange)
  {
    foreach (var (target, statType, delta) in statChange.Changes)
    {
      switch (target)
      {
        case Target.Self:
          LogAndAddToResult($"Your golem has its {statType} changed by {delta}");
          _userStats.Update(statType, delta);
          break;
        case Target.Opponent:
          LogAndAddToResult($"Your opponent has its {statType} changed by {delta}");
          _userStats.Update(statType, delta);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }

  private void LogAndAddToResult(string message)
  {
    _log.Debug(message);
    _results.Add(message);
  }
}

