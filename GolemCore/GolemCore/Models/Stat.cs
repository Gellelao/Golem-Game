using GolemCore.Models.Enums;

namespace GolemCore.Models;

public class Stat
{
  public StatType Type { get; init; }
  public int Modifier { get; init; }
  
  public NeighbourhoodRequirement? Condition { get; init; }
  
  public Neighbourhood? StatMultiplier { get; init; }
}