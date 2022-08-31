using GolemCore.Models.Enums;

namespace GolemCore.Models.Triggers;

public class StatChangeTrigger : Trigger
{
    public Target Target { get; init; }
    public StatType Stat { get; init; }
    public DeltaType DeltaType { get; init; }
    public int Threshold { get; init; }
    
    public bool Triggered(Target target, StatType statType, int delta)
    {
        if (Target != target) return false;
        if (Stat != statType) return false;
        if (Math.Abs(delta) <= Threshold) return false;
        
        switch (DeltaType)
        {
            case DeltaType.Increase:
                return delta > 0;
            case DeltaType.Decrease:
                return delta < 0;
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