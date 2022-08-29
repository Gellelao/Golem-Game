using GolemCore.Models.Enums;

namespace GolemCore.Models.Effects;

public class StatChangeEffect : Effect
{
    public Target Target { get; init; }
    public StatType Stat { get; init; }
    public int Delta { get; init; }
}