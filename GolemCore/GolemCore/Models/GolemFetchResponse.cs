namespace GolemCore.Models;

public class GolemFetchResponse
{
    public int Count { get; init; }
    public int ScannedCount { get; init; }

    public List<Golem> Items { get; init; } = new();
}