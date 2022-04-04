using System.Diagnostics.CodeAnalysis;

namespace GolemCore.Models;

public class Golem
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public DateTime Timestamp { get; init; } = DateTime.Now;

    public string UserId { get; init; }
    public int[][] PartIds { get; init; } = new int[][]
    {
        new int[3],
        new int[3],
        new int[3]
    };
}