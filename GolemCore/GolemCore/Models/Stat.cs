using GolemCore.Models.Enums;

namespace GolemCore.Models;

public class Stat
{
  public StatType Type { get; init; }
  public int Modifier { get; init; }
}