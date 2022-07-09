using GolemCore.Models;
using GolemCore.Models.Part;

namespace GolemCore;

public class Shop
{
    private readonly PartsCache _partsCache;
    private const int StartingPartCount = Constants.StartingShopPartCount;
    private readonly Random _random;

    private int _currentRound;
    private int _playerFunds;

    private readonly List<Part> _currentParts;

    public Shop(PartsCache partsCache)
    {
        _partsCache = partsCache;
        _random = new Random();
        _currentRound = 0;
        _playerFunds = Constants.StartingFunds;
        _currentParts = new List<Part>();
    }

    public List<Part> GetCurrentSelection()
    {
        return _currentParts;
    }

    public void SetPartsForRound()
    {
        var randomizedParts = _partsCache.GetAllParts().OrderBy(s => _random.NextDouble());
        
        _currentParts.AddRange(randomizedParts.Take(_currentRound+StartingPartCount));
    }

    public void IncrementRound()
    {
        _currentRound++;
    }

    public bool BuyPartAtIndex(int index)
    {
        if (index >= _currentParts.Count) return false;
        if (_playerFunds <= 0) return false;
        
        _playerFunds--;
        _currentParts.RemoveAt(index);
        return true;
    }
}