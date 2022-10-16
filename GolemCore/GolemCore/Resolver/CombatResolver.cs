using GolemCore.Models;
using GolemCore.Models.Effects;
using GolemCore.Models.Enums;
using GolemCore.Models.Golem;
using GolemCore.Models.Part;

namespace GolemCore.Resolver;

public class CombatResolver
{
  private Golem _user;
  private Golem _opponent;
  private GolemStats _userStats;
  private GolemStats _opponentStats;
  private PartsCache _cache;
  private List<string> _results;

  public CombatResolver(Golem user, Golem opponent, PartsCache partsCache)
  {
    _user = user;
    _opponent = opponent;
    _userStats = new GolemStats(user, partsCache);
    _opponentStats = new GolemStats(opponent, partsCache);
    _cache = partsCache;
    _results = new List<string>();
  }

  public List<string> GetOutcome()
  {
    var turnCounter = 0;

    _results.Add($"User stats: {_userStats}");
    _results.Add($"Opponent stats: {_opponentStats}");

    while (_userStats.Get(StatType.Health) > 0 && _opponentStats.Get(StatType.Health) > 0)
    {
      turnCounter++; // this makes the first turn "turn 1" instead of zero
      
      _results.Add($"You do {_userStats.Get(StatType.Attack)} damage to opponent");
      UpdateGolemStats(Target.Opponent, StatType.Health, _userStats.Get(StatType.Attack) * -1);
      
      _results.Add($"They do {_opponentStats.Get(StatType.Attack)} damage to you");
      UpdateGolemStats(Target.Self, StatType.Health, _opponentStats.Get(StatType.Attack) * -1);

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

        if (!effect.HasChargesLeft(golemStats.GetTriggerCount(effect)))
        {
          _results.Add($"{part.Name} effect has no charges remaining");
          break;
        }

        golemStats.IncrementEffectCount(effect);

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
      _opponentStats.Update(stat, delta); 
    }
    else
    {
      _userStats.Update(stat, delta);
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

