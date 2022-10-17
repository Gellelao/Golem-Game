using GolemCore.Models.Enums;

namespace GolemCore.Resolver;

public class MomentumTracker
{
    private readonly int _playerSpeedAdjusted;
    private readonly int _opponentSpeedAdjusted;
    private int _playerMomentum;
    private int _opponentMomentum;

    public MomentumTracker(GolemStats playerStats, GolemStats opponentStats)
    {
        var playerSpeed = playerStats.Get(StatType.Speed);
        var playerNegativeWeightFactor = playerSpeed/100 * playerStats.Get(StatType.Weight);
        _playerSpeedAdjusted = playerSpeed - playerNegativeWeightFactor;
        
        var opponentSpeed = opponentStats.Get(StatType.Speed);
        var opponentNegativeWeightFactor = opponentSpeed/100 * opponentStats.Get(StatType.Weight);
        _opponentSpeedAdjusted = opponentSpeed - opponentNegativeWeightFactor;
    }

    public Target GetNextGolem()
    {
        while (_playerMomentum < 100 && _opponentMomentum < 100)
        {
            _playerMomentum += _playerSpeedAdjusted;
            _opponentMomentum += _opponentSpeedAdjusted;
        }

        if (_playerMomentum >= 100)
        {
            _playerMomentum = 0;
            if(_opponentMomentum < 100) return Target.Self;
        }

        if (_opponentMomentum >= 100)
        {
            _opponentMomentum = 0;
            if(_playerMomentum < 100) return Target.Opponent;
        }

        return _playerMomentum > _opponentMomentum ? Target.Self : Target.Opponent;
    }
}