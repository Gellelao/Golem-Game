﻿namespace GolemCore;

public static class Constants
{
    // This one should live in the application, not this SDK
    public const string GetGolemEndpoint = "?TableName=golem";
    public const string PostEndpoint = "";
    public const string GetPartEndpoint = "?TableName=part";

    public const int TurnLimit = 20;
    public const int StartingFunds = 3;
    public const int StartingShopParts = 4;
}