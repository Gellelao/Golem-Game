namespace GolemCore.Models.Triggers;

public abstract class Trigger
{
    public abstract bool Triggered(TurnStatus turnStatus);
}