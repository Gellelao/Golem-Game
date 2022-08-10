using System.Text;
using GolemCore.Models.Enums;
using GolemCore.Models.Part;
using GolemCore.Models.Triggers;

namespace GolemCore.Extensions;

public static class PartExtensions
{
    public static string GetDescription(this Part part)
    {
        var builder = new StringBuilder();
        foreach (var stat in part.Stats)
        {
            builder.AppendLine($"{(stat.Modifier > 0 ? "+" : "")}{stat.Modifier} {stat.Type.ToString()}");
            if (stat.StatMultiplier != null)
            {
                builder.AppendLine($"For each {stat.StatMultiplier}");
            }
            if (stat.Condition != null)
            {
                builder.AppendLine($"Only if this part has {stat.Condition}");
            }
        }

        foreach (var tag in part.Tags)
        {
            builder.AppendLine(tag.ToString());
        }
        
        foreach (var trigger in part.Triggers)
        {
            builder.AppendLine(trigger.ToString());
        }

        return builder.ToString();
    }
}