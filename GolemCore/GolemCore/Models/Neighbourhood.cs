using GolemCore.Extensions;
using GolemCore.Models.Enums;

namespace GolemCore.Models;

public class Neighbourhood
{
    public Locator Locator { get; init; }
    public PartTag Tag { get; init; }

    public int MatchingNeighboursCount(string fullPartId, Golem.Golem golem, PartsCache cache)
    {
        var neighbours = new List<string>();
        switch (Locator)
        {
            case Locator.Orthogonal:
                neighbours.AddRange(golem.GetOrthogonalNeighbourIds(fullPartId));
                break;
            case Locator.Diagonal:
                throw new NotImplementedException("Haven't added a diagonal neighbour search yet");
                break;
            case Locator.Orthodiagonal:
                neighbours.AddRange(golem.GetOrthodiagonalNeighbourIds(fullPartId));
                break;
            case Locator.AllNorthernParts:
                neighbours.AddRange(golem.GetPartsNorthOf(fullPartId));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (Tag == PartTag.NotNull)
        {
            return neighbours.Count;
        }
        return neighbours.Select(n => cache.Get(n.ToPartId())).Count(p => p.Tags.Contains(Tag));
    }

    public override string ToString()
    {
        return $"{Enum.GetName(typeof(Locator), Locator)} {Enum.GetName(typeof(PartTag), Tag)} neighbour";
    }
}