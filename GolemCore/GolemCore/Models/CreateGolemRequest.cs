﻿namespace GolemCore.Models;

public class CreateGolemRequest
{
    public string TableName { get; init; } = "golem";

    public Golem Item { get; init; } = new();
}