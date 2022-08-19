using System.Text;
using GolemCore.Extensions;
using GolemCore.Models;
using GolemCore.Models.Effects;
using GolemCore.Models.Enums;
using GolemCore.Models.Golem;

namespace GolemCore;

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
  
  private class GolemStats
  {
    private readonly Golem _golem;
    private readonly PartsCache _cache;
    private readonly Dictionary<StatType, int> _currentStats;

    public GolemStats(Golem golem, PartsCache partsCache)
    {
      _golem = golem;
      _cache = partsCache;
      _currentStats = new Dictionary<StatType, int>();
      CalculateStats();
    }

    private void CalculateStats()
    {
      foreach (var statType in Enum.GetValues<StatType>())
      {
        _currentStats.Add(statType, SumStat(statType));
      }
    }

    public int SumStat(StatType typeToSum)
    {
      var sum = Constants.DefaultStats[typeToSum];
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

    public int Get(StatType stat)
    {
      return _currentStats[stat];
    }

    public void Update(StatType stat, int amount)
    {
      _currentStats[stat] += amount;
    }

    public void Reduce(StatType stat, int amount)
    {
      Update(stat, amount * -1);
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      foreach (var (stat, amount) in _currentStats)
      {
        sb.Append($"{stat}: {amount} | ");
      }

      return sb.ToString();
    }
  }
}

