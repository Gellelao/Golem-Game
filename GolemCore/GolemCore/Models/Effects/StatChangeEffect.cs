using GolemCore.Models.Enums;

namespace GolemCore.Models.Effects;

public class StatChangeEffect : Effect
{
    public Target Target { get; set; }
    public StatType Stat { get; set; }
    public int Delta { get; set; }
}