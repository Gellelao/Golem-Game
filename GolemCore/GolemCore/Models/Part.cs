using GolemCore.Models.Enums;

namespace GolemCore.Models;

public class Part
{
  public int Id { get; init; }
  public string Name { get; init; }
  public Tier Tier { get; init; }
  public PartMaterial Material { get; init; }
  public PartType Type { get; init; }
  public List<Stat> Stats { get; set; }
}