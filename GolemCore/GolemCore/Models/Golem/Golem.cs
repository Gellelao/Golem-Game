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

    public List<string> NonEmptyIdList
    {
        get { return PartIds.SelectMany(p => p).Where(id => id != "-1").ToList(); }
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