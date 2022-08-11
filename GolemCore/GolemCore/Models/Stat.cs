using GolemCore.Models.Enums;

namespace GolemCore.Models;

public class Stat
{
  public StatType Type { get; init; }
  public int Modifier { get; init; }
  
  public NeighbourhoodRequirement? Condition { get; init; }
  
  public Neighbourhood? StatMultiplier { get; init; }
  
  public int CalculateTotalModifier(string fullPartId, Golem.Golem golem, PartsCache cache)
  {
    if (Condition != null && !Condition.Satisfied(fullPartId, golem, cache))
    {
      return 0;
    }

    if (StatMultiplier != null)
    {
      return Modifier * StatMultiplier.MatchingNeighboursCount(fullPartId, golem, cache);
    }

    return Modifier;
  }
}