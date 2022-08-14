using GolemCore.Models.Enums;

namespace GolemCore.Models.Triggers;

public abstract class Trigger
{
    public Locator EffectRange { get; set; }
    // Empty indicates any tag, the more things in the list the more restrictive the trigger is
    public List<PartTag> EffectTags { get; set; } = new ();
    public abstract bool Triggered(TurnStatus turnStatus);
    public abstract override string ToString();

    public bool WouldActivate(Part.Part part)
    {
        return part.Tags.Intersect(EffectTags).Any();
    }
}