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
  public bool[][] Shape { get; init; } =
  {
    new []{true, false, false, false},
    new []{false, false, false, false},
    new []{false, false, false, false},
    new []{false, false, false, false}
  };
}