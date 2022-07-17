using GolemCore.Models;
using GolemCore.Models.Part;

namespace GolemCore;

public class Shop
{
    private readonly PartsCache _partsCache;
    private const int StartingPartCount = Constants.StartingShopPartCount;
    private readonly Random _random;

    private int _currentRound;
    public int PlayerFunds { get; private set; }

    private readonly List<Part> _currentParts;

    public Shop(PartsCache partsCache)
    {
        _partsCache = partsCache;
        _random = new Random();
        _currentRound = 0;
        PlayerFunds = Constants.StartingFunds;
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
        // In the future the buy price could vary by part, for now each part costs 1
        if (PlayerFunds <= 0) return null;
        
        PlayerFunds--;
        var partBought = _currentParts[index];
        _currentParts.RemoveAt(index);
        return partBought;
    }

    public void SellPart(Part part)
    {
        // In the future the sell price could vary by part, for now its just one
        PlayerFunds++;
    }
}