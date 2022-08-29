using GolemCore.Models.Enums;
using GolemCore.Resolver;

namespace GolemCore.Models.Triggers;

public class StatChangeTrigger : Trigger
{
    public Target Target { get; init; }
    public StatType Stat { get; init; }
    public DeltaType DeltaType { get; init; }
    public int Threshold { get; set; }
    
    public override bool Triggered(TurnStatus turnStatus)
    {
        var statChanges = turnStatus.UserStatChanges;
        if (Target == Target.Opponent)
        {
            statChanges = turnStatus.OpponentStatChanges;
        }

        if (!statChanges.ContainsKey(Stat)) return false;

        var difference = statChanges[Stat];
        switch (DeltaType)
        {
            case DeltaType.Increase:
                return difference
            case DeltaType.Decrease:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override string ToString()
    {
        throw new NotImplementedException();
    }
}