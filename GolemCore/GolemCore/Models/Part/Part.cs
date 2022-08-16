using System.Text;
using GolemCore.Models.Effects;
using GolemCore.Models.Enums;
using GolemCore.Models.Triggers;

namespace GolemCore.Models.Part;

public class Part
{
  public int Id { get; init; }
  public string Name { get; init; }
  public Tier Tier { get; init; }
  public List<PartTag> Tags { get; init; } = new();
  public List<Stat> Stats { get; init; } = new();
  public List<NeighbourhoodRequirement> Restrictions { get; init; } = new();
  public List<Trigger> Triggers { get; init; } = new();
  public List<Effect> Effects { get; init; } = new();
  public bool[][] Shape { get; init; } =
  {
    new []{true, false, false, false},
    new []{false, false, false, false},
    new []{false, false, false, false},
    new []{false, false, false, false}
  };
  
  public string GetDescription()
  {
    var builder = new StringBuilder();
    foreach (var stat in Stats)
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

    foreach (var tag in Tags)
    {
      builder.AppendLine(tag.ToString());
    }
        
    foreach (var trigger in Triggers)
    {
      builder.AppendLine(trigger.ToString());
    }

    return builder.ToString();
  }
}