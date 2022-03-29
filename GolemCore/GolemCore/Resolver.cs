using GolemCore.Models;
using GolemCore.Models.Enums;

namespace GolemCore;

public class Resolver
{
  public static List<string> GetOutcome(Golem user, Golem opponent)
  {
    var results = new List<string>();
    var userHealth = SumStat(StatType.Health, user);
    var userAttack = SumStat(StatType.Attack, user);
    var opponentHealth = SumStat(StatType.Health, opponent);
    var opponentAttack = SumStat(StatType.Attack, opponent);

    results.Add($"User HP: {userHealth}. User ATK: {userAttack}. Opponent HP: {opponentHealth}. Opponent ATK: {opponentAttack}");

    while (userHealth > 0 && opponentHealth > 0)
    {
      opponentHealth -= userAttack;
      results.Add($"You do {userAttack} damage to opponent, who is now at {opponentHealth} HP");
      userHealth -= opponentAttack;
      results.Add($"They do {opponentAttack} damage to you, leaving you at {userHealth} HP");
    }

    return results;
  }

  private static int SumStat(StatType typeToSum, Golem user)
  {
    return (from parts in user.Parts from part in parts from stat in part.Stats where stat.Type == typeToSum select stat.Modifier).Sum();
  }
}