using GolemCore.Models;
using GolemCore.Models.Golem;

namespace GolemCore.Extensions;

public static class StatExtensions
{
    public static int CalculateTotalModifier(this Stat stat, string fullPartId, Golem golem, PartsCache cache)
    {
        if (stat.Condition != null && !stat.Condition.Satisfied(fullPartId, golem, cache))
        {
            return 0;
        }

        if (stat.StatMultiplier != null)
        {
            return stat.Modifier * stat.StatMultiplier.MatchingNeighboursCount(fullPartId, golem, cache);
        }

        return stat.Modifier;
    }
}