using GolemCore.Models.Golem;

namespace GolemCore.Validation.PartRestrictions;

public class NoRestriction : PartRestriction
{
    public NoRestriction(PartsCache partsCache) : base(partsCache)
    {
    }

    public override List<ValidationProblem> GetProblems(string partIdWithSuffix, Golem golem)
    {
        return new List<ValidationProblem>();
    }
}