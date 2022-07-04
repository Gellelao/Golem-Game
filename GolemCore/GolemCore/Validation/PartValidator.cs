using GolemCore.Extensions;
using GolemCore.Models.Enums;
using GolemCore.Models.Golem;
using GolemCore.Validation.PartRestrictions;

namespace GolemCore.Validation;

public class PartValidator
{
    private readonly PartsCache _partsCache;

    public PartValidator(PartsCache partsCache)
    {
        _partsCache = partsCache;
    }

    public List<ValidationProblem> Validate(string partIdWithSuffix, Golem golem)
    {
        var problems = new List<ValidationProblem>();
        var part = _partsCache.Get(partIdWithSuffix.ToPartId());
        var tags = part.Tags;
        if (tags == null) return problems;
        foreach (var tag in tags)
        {
            var restriction = GetRestrictions(tag);
            problems.AddRange(restriction.GetProblems(partIdWithSuffix, golem));
        }

        return problems;
    }
    
    private PartRestriction GetRestrictions(PartTag tag)
    {
        return tag switch
        {
            PartTag.Grabby => new NoRestriction(_partsCache),
            PartTag.Orb => new OrbRestriction(_partsCache),
            _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, null)
        };
    }
}