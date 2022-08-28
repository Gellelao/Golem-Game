using GolemCore.Models.Enums;

namespace GolemCore.Models.Effects;

public class EffectRequirement
{
    public Target Target { get; set; }
    public StatType Stat { get; set; }
    public Comparator Comparator { get; set; }
    public int Amount { get; set; }
}