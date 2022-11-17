using GolemCore.Models.Enums;

namespace GolemCore;

public static class Constants
{
    // This one should live in the application, not this SDK
    public const string GetGolemEndpoint = "?TableName=golem";
    public const string PostEndpoint = "";
    public const string GetPartEndpoint = "?TableName=part";
    
    public const int TurnLimit = 20;
    public const int StartingFunds = 200;
    public const int StartingShopPartCount = 40; // Only a high number for testing, reduce later

    public static readonly Dictionary<StatType, int> InitialStats = new()
    {
        {StatType.Attack, 1},
        {StatType.Health, 5},
        {StatType.Speed, 5},
        {StatType.Water, 0},
        {StatType.Weight, 0},
    };
}