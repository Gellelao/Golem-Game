using GolemCore.Resolver;

namespace GolemCore.Models.Triggers;

public class TurnTrigger : Trigger
{
    public int Frequency { get; set; }
    public int StartingTurn { get; set; }
    
    public bool Triggered(int turnNumber)
    {
        return turnNumber == StartingTurn || (turnNumber - StartingTurn) % Frequency == 0;
    }

    public override string ToString()
    {
        var tags = EffectTags.Any() ? $"{string.Join(" or ", EffectTags)} " : "";
        return $"Triggers {EffectRange.ToString()} {tags}parts every {Frequency} turns, starting turn {StartingTurn}";
    }
}