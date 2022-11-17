using GolemCore.Models.Enums;

namespace GolemCore.Models;

public class StatChange
{
    public bool TargetIsPlayerGolem { get; init; }
    public StatType StatType { get; init; }
    public int Delta { get; init; }
}