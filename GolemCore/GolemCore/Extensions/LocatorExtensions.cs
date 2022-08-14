using GolemCore.Models.Enums;
using GolemCore.Models.Golem;

namespace GolemCore.Extensions;

public static class LocatorExtensions
{
    public static List<string> IdsOfNeighbouringParts(this Locator locator, string fullPartId, Golem golem)
    {
        var neighbours = new List<string>();
        switch (locator)
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

        return neighbours;
    }
}