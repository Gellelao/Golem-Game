using System.Text.Json;
using GolemCore;
using GolemCore.Models;
using GolemCore.Models.Golem;

namespace Utils;

public static class PartsManager
{
  public static async Task WritePartsFromDatabaseIntoFile(string fileName)
  {
    var client = (GolemApiClient)GolemApiClientFactory.Create();
    var parts = await client.GetParts(new CancellationToken());

    await using var fileStream = File.Create(fileName);
    await JsonSerializer.SerializeAsync(fileStream, parts, client.Options);
    await fileStream.DisposeAsync();
  }

  public static async Task UploadPartsFromFileIntoDatabase(string fileName)
  {
    
  }

  public static async Task ReplaceAllGolems(List<Golem> newGolems)
  {

  }

  public static async Task ReplaceAllParts(List<Part> newGolems)
  {

  }
}