using System.Collections;
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
  private GolemStats _userStatsBuffer;
  private GolemStats _opponentStats;
  private GolemStats _opponentStatsBuffer;
  private PartsCache _cache;
  private readonly ILogger _log;
  private List<string> _results;
  private Queue<StatChange> _statChangeQueue;

  public CombatResolver(Golem user, Golem opponent, PartsCache partsCache)
  {
    _user = user;
    _opponent = opponent;
    _userStats = new GolemStats(user, partsCache);
    _userStatsBuffer = new GolemStats(_userStats);
    _opponentStats = new GolemStats(opponent, partsCache);
    _opponentStatsBuffer = new GolemStats(_opponentStats);
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

    _results.Add($"User stats: {_userStats}");
    _results.Add($"Opponent stats: {_opponentStats}");

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
          _statChangeQueue.Enqueue(new StatChange
          {
            Changes = new List<(Target, StatType, int)>
            {
              (Target.Opponent, StatType.Health, _userStats.Get(StatType.Attack) * -1)
            }
          });
          //UserAttackOpponent();
          break;
        case AttackOrder.Opponent:
          _statChangeQueue.Enqueue(new StatChange
          {
            Changes = new List<(Target, StatType, int)>
            {
              (Target.User, StatType.Health, _opponentStats.Get(StatType.Attack) * -1)
            }
          });
          //OpponentAttackUser();
          break;
        case AttackOrder.Simultaneous:
          _statChangeQueue.Enqueue(new StatChange
          {
            Changes = new List<(Target, StatType, int)>
            {
              (Target.User, StatType.Health, _opponentStats.Get(StatType.Attack) * -1),
              (Target.Opponent, StatType.Health, _userStats.Get(StatType.Attack) * -1)
            }
          });
          // This is where we need to make sure that stat changes from this turn don't affect the following attack
          // Need an "applyStatChanges" method or something to commit changes once we're done with combat
          //UserAttackOpponent();
          //OpponentAttackUser();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      while (_statChangeQueue.Any())
      {
        var statChange = _statChangeQueue.Dequeue();
        ApplyStatChanges(statChange);
        var userActivatedParts = new List<Part>();
        var opponentActivatedParts = new List<Part>();
        foreach (var (target, statType, delta) in statChange.Changes)
        {
          switch (target)
          {
            case Target.User:
              userActivatedParts.AddRange(_user.GetPartsActivatedByStatChange(statChange, _cache));
              break;
            case Target.Opponent:
              opponentActivatedParts.AddRange(_opponent.GetPartsActivatedByStatChange(statChange, _cache));
              break;
            default:
              throw new ArgumentOutOfRangeException();
          }
        }

        List<(Target, StatType, int)> changesFromEffects = new List<(Target, StatType, int)>();
        foreach (var part in userActivatedParts)
        {
          _results.Add($"{part.Name} activated!");
          foreach (var effect in part.Effects)
          {
            if (!effect.RequirementMet(_userStats, _opponentStats))
            {
              _results.Add($"{part.Name} effect requirement is not met");
              break;
            }

            if (!effect.HasChargesLeft(_userStats.GetEffectTriggerCount(effect)))
            {
              _results.Add($"{part.Name} effect has no charges remaining");
              break;
            }

            _userStats.IncrementEffectTriggerCount(effect);

            switch (effect)
            {
              case StatChangeEffect statChangeEffect:
                changesFromEffects.Add((statChangeEffect.Target, statChangeEffect.Stat, statChangeEffect.Delta));
                //UpdateGolemStats(statChangeEffect.Target, statChangeEffect.Stat, statChangeEffect.Delta);
                _results.Add($"Effect has changed {statChangeEffect.Target} {statChangeEffect.Stat} by {statChangeEffect.Delta}! Now at {(statChangeEffect.Target == Target.Opponent ? _opponentStats.Get(StatType.Health) : _userStats.Get(StatType.Health))}");
                break;
            }
          }
        }
        _statChangeQueue.Enqueue(new StatChange
        {
          Changes = changesFromEffects
        });
      }

      // All stat WRITING must go to the buffers
      // All stat READING must go to the real stat objects
      // Only apply the changes from the buffer to the real stats after all attacks are completed for the turn
      ProcessTurnTriggers(_user, _userStats, turnCounter);
      ProcessTurnTriggers(_opponent, _opponentStats, turnCounter);

      _results.Add($"--------------turn {turnCounter} over");
      
      if (turnCounter >= Constants.TurnLimit)
      {
        _results.Add($"Turn limit has been reached after {turnCounter} turns. Game is a draw!");
        break;
      }
    }

    return _results;
  }

  private void ApplyStatChanges(StatChange statChange)
  {
    foreach (var (target, statType, delta) in statChange.Changes)
    {
      switch (target)
      {
        case Target.User:
          _userStats.Update(statType, delta);
          break;
        case Target.Opponent:
          _userStats.Update(statType, delta);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }


  private void UserAttackOpponent()
  {
    _results.Add($"You do {_userStats.Get(StatType.Attack)} damage to opponent");
    UpdateGolemStats(Target.Opponent, StatType.Health, _userStats.Get(StatType.Attack) * -1);
  }

  private void OpponentAttackUser()
  {
    _results.Add($"They do {_opponentStats.Get(StatType.Attack)} damage to you");
    UpdateGolemStats(Target.User, StatType.Health, _opponentStats.Get(StatType.Attack) * -1);
  }

  private void ProcessTurnTriggers(Golem golem, GolemStats golemStats, int turnNumber)
  {
    var userActivatedParts = golem.GetPartsActivatedByTurn(turnNumber, _cache);
    ProcessEffects(golemStats, userActivatedParts);
  }

  private void ProcessEffects(GolemStats golemStats, List<Part> activatedParts)
  {
    foreach (var part in activatedParts)
    {
      _results.Add($"{part.Name} activated!");
      foreach (var effect in part.Effects)
      {
        if (!effect.RequirementMet(_userStats, _opponentStats))
        {
          _results.Add($"{part.Name} effect requirement is not met");
          break;
        }

        if (!effect.HasChargesLeft(golemStats.GetEffectTriggerCount(effect)))
        {
          _results.Add($"{part.Name} effect has no charges remaining");
          break;
        }

        golemStats.IncrementEffectTriggerCount(effect);

        switch (effect)
        {
          case StatChangeEffect statChangeEffect:
            UpdateGolemStats(statChangeEffect.Target, statChangeEffect.Stat, statChangeEffect.Delta);
            _results.Add($"Effect has changed {statChangeEffect.Target} {statChangeEffect.Stat} by {statChangeEffect.Delta}! Now at {(statChangeEffect.Target == Target.Opponent ? _opponentStats.Get(StatType.Health) : _userStats.Get(StatType.Health))}");
            break;
        }
      }
    }
  }

  private void UpdateGolemStats(Target target, StatType stat, int delta)
  {
    if (target == Target.Opponent)
    {
      _opponentStatsBuffer.Update(stat, delta); 
    }
    else
    {
      _userStatsBuffer.Update(stat, delta);
    }
    ProcessStatChangeTriggers(_user, _userStats, target, stat, delta);
    ProcessStatChangeTriggers(_opponent, _opponentStats, target, stat, delta);
  }
  
  private void ProcessStatChangeTriggers(Golem golem, GolemStats golemStats, Target target, StatType statType, int delta)
  {
    var userActivatedParts = golem.GetPartsActivatedByStatChange(target, statType, delta, _cache);
    ProcessEffects(golemStats, userActivatedParts);
  }
}

