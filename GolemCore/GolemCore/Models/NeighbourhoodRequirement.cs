using GolemCore.Extensions;
using GolemCore.Models.Enums;

namespace GolemCore.Models;

public class NeighbourhoodRequirement : Neighbourhood
{
    public Requirement Requirement { get; init; }

    public bool Satisfied(string fullPartId, Golem.Golem golem, PartsCache cache)
    {
        var matchingNeighbours = MatchingNeighboursCount(fullPartId, golem, cache);
        switch (Requirement)
        {
            case Requirement.OneOrMore:
                return matchingNeighbours > 0;
            case Requirement.Zero:
                return matchingNeighbours == 0;
            case Requirement.One:
                return matchingNeighbours == 1;
            case Requirement.Two:
                return matchingNeighbours == 2;
            case Requirement.Three:
                return matchingNeighbours == 3;
            case Requirement.Four:
                return matchingNeighbours == 4;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override string ToString()
    {
        return
            $"{Requirement.ToString()} {Locator.ToString()} {Tag.ToString()} neighbours";
    }

    public string UnsatisfiedMessage(string partName)
    {
        return
            $"{partName} does not have {this}";
    }
}