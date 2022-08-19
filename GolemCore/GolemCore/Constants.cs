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
    public const int StartingShopPartCount = 4;

    public static Dictionary<StatType, int> DefaultStats = new()
    {
        {StatType.Attack, 1},
        {StatType.Health, 5},
        {StatType.Speed, 0},
        {StatType.Water, 0},
    };
}