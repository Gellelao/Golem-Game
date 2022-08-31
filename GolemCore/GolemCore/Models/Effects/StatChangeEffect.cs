using GolemCore.Models.Enums;

namespace GolemCore.Models.Effects;

public class StatChangeEffect : Effect
{
    public Target Target { get; init; }
    public StatType Stat { get; init; }
    public int Delta { get; init; }

    public override string ToString()
    {
        var description = $"Effect: Alter {Target} {Stat} by {Delta}";
        if(Requirement != null) description += $"\n{Requirement}";
        if (Charges > 0) description += $"\nCan trigger {Charges} times per battle";
        return description;
    }
}