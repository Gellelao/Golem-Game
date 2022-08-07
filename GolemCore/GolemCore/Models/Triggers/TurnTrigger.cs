namespace GolemCore.Models.Triggers;

public class TurnTrigger : Trigger
{
    public int Frequency { get; set; }
    public int StartingTurn { get; set; }
}