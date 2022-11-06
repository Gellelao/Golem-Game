using GolemCore.Models;
using GolemCore.Models.Effects;
using GolemCore.Models.Enums;

namespace GolemCore;

public static class CommonPartProperties
{
    public static bool[][] TwoByTwoTileShape { get; } =
    {
        new[] {true, true, false, false},
        new[] {true, true, false, false},
        new[] {false, false, false, false},
        new[] {false, false, false, false}
    };

    public static EffectRequirement GreaterThanZeroWater { get; } = new()
    {
        Target = Target.User,
        Stat = StatType.Water,
        Comparator = Comparator.GreaterThan,
        Amount = 0
    };
}