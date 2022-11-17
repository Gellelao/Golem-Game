using GolemCore.Models.Enums;
using GolemCore.Resolver;

namespace GolemCore.Models.Triggers;

public class StatChangeTrigger : Trigger
{
    public Target Target { get; init; }
    public StatType Stat { get; init; }
    public DeltaType DeltaType { get; init; }
    public int Threshold { get; init; }
    
    public bool Triggered(StatChange statChange, Target target)
    {
        if (Target != target) return false;
        if (Stat != statChange.StatType) return false;
        if (Math.Abs(statChange.Delta) <= Threshold) return false;
        
        switch (DeltaType)
        {
            case DeltaType.Increase:
                return statChange.Delta > 0;
            case DeltaType.Decrease:
                return statChange.Delta < 0;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override string ToString()
    {
        var tags = EffectTags.Any() ? $"{string.Join(" or ", EffectTags)} " : "";
        var locator = EffectRange == Locator.Self ? "self" : $"{EffectRange.ToString()} {tags}parts";
        var description = $"Triggers {locator} when {Target} {Stat} {DeltaType}s";
        if (Threshold > 0) description += $" by more than {Threshold}";
        return description;
    }
}