using GolemCore.Models;
using GolemCore.Models.Part;

namespace GolemCore;

public class PartsCache
{
  private Dictionary<int, Part> parts;

  public PartsCache(List<Part> partsList)
  {
    parts = new Dictionary<int, Part>();
    foreach (var part in partsList)
    {
      parts.Add(part.Id, part);
    }
  }

  public Part Get(int partId)
  {
    var success = parts.TryGetValue(partId, out var part);
    if (success)
    {
      return part;
    }

    throw new KeyNotFoundException($"Part {partId} was not found in the cache");
  }

  public IEnumerable<Part> GetAllParts()
  {
    return parts.Values.ToList();
  }
}