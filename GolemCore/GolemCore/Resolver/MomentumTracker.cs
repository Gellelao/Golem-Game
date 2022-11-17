using GolemCore.Models.Enums;
using Serilog;

namespace GolemCore.Resolver;

public class MomentumTracker
{
    private readonly ILogger _log;
    private readonly double _userSpeedAdjusted;
    private readonly double _opponentSpeedAdjusted;
    private double _userMomentum;
    private double _opponentMomentum;

    public MomentumTracker(GolemStats userStats, GolemStats opponentStats, ILogger log)
    {
        _log = log;
        var playerSpeed = (double)userStats.Get(StatType.Speed);
        double playerNegativeWeightFactor = playerSpeed/100 * userStats.Get(StatType.Weight);
        _userSpeedAdjusted = playerSpeed - playerNegativeWeightFactor;
        
        var opponentSpeed = (double)opponentStats.Get(StatType.Speed);
        double opponentNegativeWeightFactor = opponentSpeed/100 * opponentStats.Get(StatType.Weight);
        _opponentSpeedAdjusted = opponentSpeed - opponentNegativeWeightFactor;
    }

    public AttackOrder GetNextGolem()
    {
        while (_userMomentum < 100 && _opponentMomentum < 100)
        {
            _userMomentum += _userSpeedAdjusted;
            _opponentMomentum += _opponentSpeedAdjusted;
            _log.Debug("User momentum    : {a} | Opponent momentum: {b}", _userMomentum, _opponentMomentum);
        }

        if (_userMomentum == _opponentMomentum)
        {
            _opponentMomentum = 0;
            _userMomentum = 0;
            return AttackOrder.Simultaneous;
        }

        if (_userMomentum >= 100)
        {
            _userMomentum = 0;
            if(_opponentMomentum < 100) return AttackOrder.User;
        }

        if (_opponentMomentum >= 100)
        {
            _opponentMomentum = 0;
            if(_userMomentum < 100) return AttackOrder.Opponent;
        }
        
        return _userMomentum > _opponentMomentum ? AttackOrder.User : AttackOrder.Opponent;
    }
}