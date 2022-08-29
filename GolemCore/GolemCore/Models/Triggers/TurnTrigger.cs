using GolemCore.Resolver;

namespace GolemCore.Models.Triggers;

public class TurnTrigger : Trigger
{
    public int Frequency { get; set; }
    public int StartingTurn { get; set; }
    public override bool Triggered(TurnStatus turnStatus)
    {
        var turnCounter = turnStatus.TurnCounter;
        return turnCounter == StartingTurn || (turnCounter - StartingTurn) % Frequency == 0;
    }

    public override string ToString()
    {
        var tags = EffectTags.Any() ? $"{string.Join(" or ", EffectTags)} " : "";
        return $"Triggers {EffectRange.ToString()} {tags}parts every {Frequency} turns, starting turn {StartingTurn}";
    }
}