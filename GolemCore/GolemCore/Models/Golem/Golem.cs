using System.Diagnostics.CodeAnalysis;

namespace GolemCore.Models.Golem;

public class Golem
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public DateTime Timestamp { get; init; } = DateTime.Now;

    public int UserId { get; init; }
    public int Version { get; init; } = 1;
    public int[][] PartIds { get; init; } = new int[][]
    {
        new int[3],
        new int[3],
        new int[3]
    };
}