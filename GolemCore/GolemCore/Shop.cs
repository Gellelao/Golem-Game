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

    public Part? BuyPartAtIndex(int index)
    {
        if (index >= _currentParts.Count) return null;
        if (_playerFunds <= 0) return null;
        
        _playerFunds--;
        var partBought = _currentParts[index];
        _currentParts.RemoveAt(index);
        return partBought;
    }
}