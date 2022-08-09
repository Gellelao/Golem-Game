using GolemCore.Models.Enums;

namespace GolemCore.Models;

public class TurnStatus
{
    public int TurnCounter { get; init; }
    public Dictionary<StatType, int> UserStatChanges { get; } = new();
    public Dictionary<StatType, int> OpponentStatChanges { get; } = new();
    public TurnStatus(int turnCounter)
    {
        TurnCounter = turnCounter;
    }
}