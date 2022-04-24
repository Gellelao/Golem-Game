using GolemCore.Models;
using GolemCore.Models.Enums;
using GolemCore.Models.Golem;

namespace GolemCore;

public class Resolver
{
  public static List<string> GetOutcome(Golem user, Golem opponent, PartsCache partsCache)
  {
    var results = new List<string>();
    var userParser = new GolemParser(user, partsCache);
    var opponentParser = new GolemParser(opponent, partsCache);
    var userHealth = userParser.SumStat(StatType.Health);
    var userAttack = userParser.SumStat(StatType.Attack);
    var opponentHealth = opponentParser.SumStat(StatType.Health);
    var opponentAttack = opponentParser.SumStat(StatType.Attack);

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
  
  private class GolemParser
  {
    private readonly Golem _golem;
    private readonly PartsCache _cache;
    
    public GolemParser(Golem golem, PartsCache partsCache)
    {
      _golem = golem;
      _cache = partsCache;
    }

    public int SumStat(StatType typeToSum)
    {
      var sum = 0;
      List<string> seenParts = new List<string>();
      foreach (var idList in _golem.PartIds)
      {
        foreach (var id in idList.Where(id => id != "-1"))
        {
          int idForLookup;
          if (seenParts.Contains(id)) continue;
          if (id.Contains('.'))
          {
            seenParts.Add(id);
            idForLookup = int.Parse(id.Split('.')[0]);
          }
          else
          {
            idForLookup = int.Parse(id);
          }
          var part = _cache.Get(idForLookup);
          sum += part.Stats.Where(stat => stat.Type == typeToSum).Sum(stat => stat.Modifier);
        }
      }

      return sum;
    }
  }
}

