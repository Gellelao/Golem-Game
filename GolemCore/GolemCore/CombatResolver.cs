using GolemCore.Extensions;
using GolemCore.Models;
using GolemCore.Models.Enums;
using GolemCore.Models.Golem;

namespace GolemCore;

public class CombatResolver
{
  public static List<string> GetOutcome(Golem user, Golem opponent, PartsCache partsCache)
  {
    var turnCounter = 0;
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
      turnCounter++; // this makes the first turn "turn 1" instead of zero
      var turnStatus = new TurnStatus(turnCounter);
      opponentHealth -= userAttack;
      results.Add($"You do {userAttack} damage to opponent, who is now at {opponentHealth} HP");
      userHealth -= opponentAttack;
      results.Add($"They do {opponentAttack} damage to you, leaving you at {userHealth} HP");
      var userTriggersTriggered = user.NonEmptyIdList.Select(id => partsCache.Get(id.ToPartId()))
        .Where(part => part.Triggers.Any(t => t.Triggered(turnStatus)));
      results.AddRange(userTriggersTriggered.Select(part => $"{part.Name} triggered!"));
      
      if (turnCounter >= Constants.TurnLimit)
      {
        results.Add($"Turn limit has been reached after {turnCounter} turns. Game is a draw!");
        break;
      }
      results.Add($"--------------turn {turnCounter} over");
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
          if (seenParts.Contains(id)) continue;
          seenParts.Add(id);
          var part = _cache.Get(id.ToPartId());
          sum += part.Stats.Where(s => s.Type == typeToSum).Sum(stat => stat.CalculateTotalModifier(id, _golem, _cache));
        }
      }

      return sum;
    }
  }
}

