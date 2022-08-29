using GolemCore.Models.Enums;

namespace GolemCore.Resolver;

public class StatChangeArgs : EventArgs
{
    public StatType StatType { get; }
    public int Delta { get; }
    
    public StatChangeArgs(StatType statType, int delta)
    {
        StatType = statType;
        Delta = delta;
    }
}