using GolemCore.Extensions;
using GolemCore.Models.Enums;

namespace GolemCore.Models;

public class Neighbourhood
{
    public Locator Locator { get; init; }
    public PartTag Tag { get; init; }

    public int MatchingNeighboursCount(string fullPartId, Golem.Golem golem, PartsCache cache)
    {
        var neighbours = golem.GetNeighbourIds(fullPartId, Locator);
        
        if (Tag == PartTag.NotNull)
        {
            return neighbours.Count;
        }
        return neighbours.Select(n => cache.Get(n.ToPartId())).Count(p => p.Tags.Contains(Tag));
    }

    public override string ToString()
    {
        return $"{Locator.ToString()} {Tag.ToString()} neighbours";
    }
}