using GolemCore.Models;

namespace GolemCore;

public static class PartsCache
{
  private static Dictionary<int, Part>? parts = null;

  public static Part Get(int partId)
  {
    await PopulateCacheIfNull();
    var success = parts.TryGetValue(partId, out var part);
    if (success)
    {
      return part;
    }

    throw new KeyNotFoundException($"Part {partId} was not found in the cache");
  }

  private static async Task PopulateCacheIfNull()
  {
    if (parts == null)
    {
      parts = new Dictionary<int, Part>();

      var client = GolemApiClientFactory.Create();
      var partsResponse = await client.GetParts(new CancellationToken());

      foreach (var part in partsResponse)
      {

      }
    }
  }
}