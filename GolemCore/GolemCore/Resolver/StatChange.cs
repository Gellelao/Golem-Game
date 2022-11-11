using GolemCore.Models.Enums;

namespace GolemCore.Resolver;

public class StatChange
{
    public bool TargetsPlayerGolem { get; init; }
    public bool TargetsSelf { get; init; }
    public StatType StatType { get; init; }
    public int Delta { get; init; }
}