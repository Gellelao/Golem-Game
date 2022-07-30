using System.Text.Json;
using GolemCore;
using GolemCore.Models;
using GolemCore.Models.Golem;
using GolemCore.Models.Part;

namespace Utils;

public static class PartsManager
{
  public static async Task WritePartsFromDatabaseIntoFile(string fileName)
  {
    var client = (GolemApiClient)GolemApiClientFactory.Create();
    var parts = await client.GetParts(new CancellationToken());

    var options = client.Options;
    options.WriteIndented = true;

    await using var fileStream = File.Create(fileName);
    await JsonSerializer.SerializeAsync(fileStream, parts, options);
    await fileStream.DisposeAsync();
  }

  public static async Task PostAllParts(List<Part> parts)
  {
    var client = (GolemApiClient)GolemApiClientFactory.Create();
    foreach (var part in parts)
    {
      await client.PostPart(new CreatePartRequest
      {
        Item = part
      }, new CancellationToken());
    }
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