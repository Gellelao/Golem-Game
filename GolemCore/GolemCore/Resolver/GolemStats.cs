using System.Text;
using GolemCore.Extensions;
using GolemCore.Models.Enums;
using GolemCore.Models.Golem;

namespace GolemCore.Resolver;

public class GolemStats
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