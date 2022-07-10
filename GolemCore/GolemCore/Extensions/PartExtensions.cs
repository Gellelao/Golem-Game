using System.Text;
using GolemCore.Models.Enums;
using GolemCore.Models.Part;

namespace GolemCore.Extensions;

public static class PartExtensions
{
    public static string GetDescription(this Part part)
    {
        var builder = new StringBuilder();
        foreach (var stat in part.Stats)
        {
            builder.AppendLine($"{(stat.Modifier > 0 ? "+" : "")}{stat.Modifier} {Enum.GetName(typeof(StatType), stat.Type)}");
        }

        if(part.Tags == null)        return builder.ToString();
        foreach (var tag in part.Tags)
        {
            builder.AppendLine(tag.GetDescription());
        }

        return builder.ToString();
    }
}