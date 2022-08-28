using GolemCore.Models.Enums;
using GolemCore.Resolver;

namespace GolemCore.Models.Effects;

public abstract class Effect
{
    public EffectRequirement? Requirement { get; set; }
    
    // No part should have zero charges so zero indicates infinite
    public int Charges { get; set; } = 0;

    public bool RequirementMet(GolemStats user, GolemStats opponent)
    {
        if (Requirement == null) return true;
        var golemToCheck = user;
        if (Requirement.Target == Target.Opponent)
        {
            golemToCheck = opponent;
        }

        return Requirement.Comparator switch
        {
            Comparator.LessThan => golemToCheck.Get(Requirement.Stat) < Requirement.Amount,
            Comparator.GreaterThan => golemToCheck.Get(Requirement.Stat) > Requirement.Amount,
            Comparator.EqualTo => golemToCheck.Get(Requirement.Stat) == Requirement.Amount,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}