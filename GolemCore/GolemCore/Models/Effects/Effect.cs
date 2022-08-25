namespace GolemCore.Models.Effects;

public abstract class Effect
{
    public EffectRequirement Requirement { get; set; }
    public int Charges { get; set; }
}