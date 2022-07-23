using GolemCore.Models.Part;

namespace GolemCore;

public class Shop
{
    private readonly PartsCache _partsCache;
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
        _currentParts.Clear();
        // could include _currentRound here to display more parts as rounds progress
        _currentParts.AddRange(randomizedParts.Take(Constants.StartingShopPartCount));
    }

    public void IncrementRound()
    {
        _currentRound++;
    }

    public void IncrementFunds()
    {
        PlayerFunds++;
    }

    public bool BuyPartAtIndex(int index)
    {
        if (index >= _currentParts.Count) return false;
        // In the future the buy price could vary by part, for now each part costs 1
        if (PlayerFunds <= 0) return false;
        
        PlayerFunds--;
        _currentParts.RemoveAt(index);
        return true;
    }

    public void SellPart(Part part)
    {
        // In the future the sell price could vary by part, for now its just one
        PlayerFunds++;
    }

    public bool CanAfford(Part part)
    {
        return PlayerFunds > 0;
    }
}