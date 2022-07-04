using GolemCore.Models;
using GolemCore.Models.Part;

namespace GolemCore;

public class Shop
{
    private readonly PartsCache _partsCache;
    private const int StartingParts = Constants.StartingShopParts;
    private readonly Random _random;

    private int _currentRound;
    private int _playerFunds;

    public Shop(PartsCache partsCache)
    {
        _partsCache = partsCache;
        _random = new Random();
        _currentRound = 0;
        _playerFunds = Constants.StartingFunds;
    }

    public List<Part> GetPartsForRound()
    {
        var randomizedParts = _partsCache.GetAllParts().OrderBy(s => _random.NextDouble());
        var partsToDisplay = new List<Part>();
        
        partsToDisplay.AddRange(randomizedParts.Take(_currentRound+StartingParts));

        return partsToDisplay;
    }

    public void IncrementRound()
    {
        _currentRound++;
    }
}