using GolemCore.Models.Enums;

namespace GolemCore.Extensions;

public static class PartTagExtensions
{
    public static string GetDescription(this PartTag tag)
    {
        return tag switch
        {
            PartTag.Grabby => "Grabby",
            PartTag.Orb => "Must be adjacent to a Grabby part",
            _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, null)
        };
    }
}