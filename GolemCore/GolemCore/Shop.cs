using GolemCore.Models;

namespace GolemCore;

public class Shop
{
    private readonly PartsCache _partsCache;
    private const int StartingParts = 2;
    private readonly Random _random;

    public Shop(PartsCache partsCache)
    {
        _partsCache = partsCache;
        _random = new Random();
    }

    public List<Part> GetPartsForRound(int roundNumber)
    {
        var randomizedParts = _partsCache.GetAllParts().OrderBy(s => _random.NextDouble());
        var partsToDisplay = new List<Part>();
        
        partsToDisplay.AddRange(randomizedParts.Take(roundNumber+StartingParts));

        return partsToDisplay;
    }
}