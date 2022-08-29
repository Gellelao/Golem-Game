using GolemCore.Models;
using GolemCore.Models.Effects;
using GolemCore.Models.Enums;
using GolemCore.Models.Golem;

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
      var turnStatus = new TurnStatus(turnCounter);
      
      _opponentStats.Reduce(StatType.Health, _userStats.Get(StatType.Attack));
      _results.Add($"You do {_userStats.Get(StatType.Attack)} damage to opponent, who is now at {_opponentStats.Get(StatType.Health)} HP");
      
      _userStats.Reduce(StatType.Health, _opponentStats.Get(StatType.Attack));
      _results.Add($"They do {_opponentStats.Get(StatType.Attack)} damage to you, leaving you at {_userStats.Get(StatType.Health)} HP");

      var userActivatedParts = _user.GetActivatedParts(turnStatus, _cache);
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

          if (!effect.HasChargesLeft(_userStats.GetTriggerCount(effect)))
          {
            _results.Add($"{part.Name} effect has no charges remaining");
            break;
          }
          
          _userStats.IncrementEffectCount(effect);
          
          switch (effect)
          {
            case StatChangeEffect statChangeEffect:
              if (statChangeEffect.Target == Target.Opponent)
              {
                _opponentStats.Update(statChangeEffect.Stat, statChangeEffect.Delta);
                _results.Add($"{part.Name} effect has changed opponent {statChangeEffect.Stat} by {statChangeEffect.Delta}! Now at {_opponentStats.Get(StatType.Health)}");
              }
              else
              {
                _userStats.Update(statChangeEffect.Stat, statChangeEffect.Delta);
                _results.Add($"{part.Name} effect has changed user {statChangeEffect.Stat} by {statChangeEffect.Delta}! Now at {_userStats.Get(StatType.Health)}");
              }

              break;
          }
        }
      }
      
      if (turnCounter >= Constants.TurnLimit)
      {
        _results.Add($"Turn limit has been reached after {turnCounter} turns. Game is a draw!");
        break;
      }
      _results.Add($"--------------turn {turnCounter} over");
    }

    return _results;
  }
}

