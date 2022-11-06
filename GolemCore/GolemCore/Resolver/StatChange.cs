using GolemCore.Models.Enums;

namespace GolemCore.Resolver;

public class StatChange
{
    public List<(Target, StatType, int)> Changes = new();
}