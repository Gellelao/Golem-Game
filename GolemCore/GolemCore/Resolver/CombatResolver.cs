using GolemCore.Models;
using GolemCore.Models.Effects;
using GolemCore.Models.Enums;
using GolemCore.Models.Golem;

namespace GolemCore.Resolver;

public class CombatResolver
{
  public static List<string> GetOutcome(Golem user, Golem opponent, PartsCache partsCache)
  {
    var turnCounter = 0;
    var results = new List<string>();
    var userStats = new GolemStats(user, partsCache);
    var opponentStats = new GolemStats(opponent, partsCache);

    results.Add($"User stats: {userStats}");
    results.Add($"Opponent stats: {opponentStats}");

    while (userStats.Get(StatType.Health) > 0 && opponentStats.Get(StatType.Health) > 0)
    {
      turnCounter++; // this makes the first turn "turn 1" instead of zero
      var turnStatus = new TurnStatus(turnCounter);
      
      opponentStats.Reduce(StatType.Health, userStats.Get(StatType.Attack));
      results.Add($"You do {userStats.Get(StatType.Attack)} damage to opponent, who is now at {opponentStats.Get(StatType.Health)} HP");
      
      userStats.Reduce(StatType.Health, opponentStats.Get(StatType.Attack));
      results.Add($"They do {opponentStats.Get(StatType.Attack)} damage to you, leaving you at {userStats.Get(StatType.Health)} HP");

      var userActivatedParts = user.GetActivatedParts(turnStatus, partsCache);
      foreach (var part in userActivatedParts)
      {
        results.Add($"{part.Name} activated!");
        foreach (var effect in part.Effects)
        {
          if (!effect.RequirementMet(userStats, opponentStats))
          {
            results.Add($"{part.Name} effect requirement is not met");
            break;
          }

          if (!effect.HasChargesLeft(userStats.GetTriggerCount(effect)))
          {
            results.Add($"{part.Name} effect has no charges remaining");
            break;
          }
          
          userStats.IncrementEffectCount(effect);
          
          switch (effect)
          {
            case StatChangeEffect statChangeEffect:
              if (statChangeEffect.Target == Target.Opponent)
              {
                opponentStats.Update(statChangeEffect.Stat, statChangeEffect.Delta);
                results.Add($"{part.Name} effect has changed opponent {statChangeEffect.Stat} by {statChangeEffect.Delta}! Now at {opponentStats.Get(StatType.Health)}");
              }
              else
              {
                userStats.Update(statChangeEffect.Stat, statChangeEffect.Delta);
                results.Add($"{part.Name} effect has changed user {statChangeEffect.Stat} by {statChangeEffect.Delta}! Now at {userStats.Get(StatType.Health)}");
              }

              break;
          }
        }
      }
      
      if (turnCounter >= Constants.TurnLimit)
      {
        results.Add($"Turn limit has been reached after {turnCounter} turns. Game is a draw!");
        break;
      }
      results.Add($"--------------turn {turnCounter} over");
    }

    return results;
  }
}

