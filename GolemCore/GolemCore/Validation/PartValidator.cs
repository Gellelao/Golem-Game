using GolemCore.Extensions;
using GolemCore.Models.Golem;

namespace GolemCore.Validation;

public class PartValidator
{
    private readonly PartsCache _partsCache;

    public PartValidator(PartsCache partsCache)
    {
        _partsCache = partsCache;
    }

    public List<ValidationProblem> Validate(string fullPartId, Golem golem)
    {
        var problems = new List<ValidationProblem>();
        var part = _partsCache.Get(fullPartId.ToPartId());
        foreach (var restriction in part.Restrictions)
        {
            if (!restriction.Satisfied(fullPartId, golem, _partsCache))
            {
                problems.Add(new ValidationProblem(restriction.UnsatisfiedMessage(part.Name)));
            }
        }

        return problems;
    }
}