using GolemCore.Models.Enums;
using GolemCore.Resolver;

namespace GolemCore.Models.Triggers;

public abstract class Trigger
{
    public Locator EffectRange { get; set; }
    // Empty indicates any tag, the more things in the list the more restrictive the trigger is
    public List<PartTag> EffectTags { get; set; } = new ();
    public abstract override string ToString();

    public bool WouldActivate(Part.Part part)
    {
        if (!EffectTags.Any()) return true;
        return part.Tags.Intersect(EffectTags).Any();
    }
}