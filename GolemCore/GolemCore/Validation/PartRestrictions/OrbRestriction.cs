using GolemCore.Models.Enums;
using GolemCore.Models.Golem;

namespace GolemCore.Validation.PartRestrictions;

public class OrbRestriction : PartRestriction
{
    public OrbRestriction(PartsCache partsCache) : base(partsCache)
    {
    }

    public override List<ValidationProblem> GetProblems(string partIdWithSuffix, Golem golem)
    {
        var problems = new List<ValidationProblem>();
        if (!AnyNeighboursWithTag(partIdWithSuffix, golem, PartTag.Grabby))
        {
            problems.Add(new ValidationProblem("Orb must be adjacent to a Grabby part"));
        }

        return problems;
    }
}