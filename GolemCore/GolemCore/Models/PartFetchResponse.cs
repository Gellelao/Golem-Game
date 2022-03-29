﻿namespace GolemCore.Models;

public class PartFetchResponse
{
    public int Count { get; init; }
    public int ScannedCount { get; init; }

    public List<Part> Items { get; init; } = new();
}