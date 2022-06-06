using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GolemCore.Models.Golem;

public class Golem
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public DateTime Timestamp { get; init; } = DateTime.Now;

    public int UserId { get; init; }
    public int Version { get; init; } = 1;
    public string[][] PartIds { get; init; } =
    {
        new []{"-1","-1","-1","-1"},
        new []{"-1","-1","-1","-1"},
        new []{"-1","-1","-1","-1"},
        new []{"-1","-1","-1","-1"}
    };

    public int GetHighestExistingCount(int partId)
    {
        var existingPartsWithSameId = PartIds.SelectMany(p => p).
            Select(id => id.Split('.')).
            Where(tuple => tuple[0] == partId.ToString()).
            ToList();
        if (!existingPartsWithSameId.Any())
        {
            Console.WriteLine($"No parts with id {partId}, returning 0");
            return 0;
        }

        var partsWithSuffixes = existingPartsWithSameId.Where(tuple => tuple.Length > 1).ToList();
        var result = partsWithSuffixes.Any()
            ? partsWithSuffixes.Select(tuple => int.Parse(tuple[1])).OrderByDescending(x => x).First() + 1
            : 1;
        Console.WriteLine(result);
        return result;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"User {UserId}");
        foreach (var line in PartIds)
        {
            builder.AppendJoin(", ", line);
            builder.AppendLine();
        }

        return builder.ToString();
    }
}