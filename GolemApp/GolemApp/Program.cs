using System.Text.Json;
using GolemApp;
using GolemCore;
using GolemCore.Models;
using GolemCore.Models.Golem;

Console.WriteLine("Hello, World!");

var client = GolemApiClientFactory.Create();

var view = new ConsoleView(new ConsoleWrapper());

var cancellationToken = new CancellationToken();

var golem1 = new Golem
{
    UserId = 1,
    PartIds = new []
    {
        new []{1, 1, 1},
        new []{0, 0, 0},
        new []{1, 0, 1},
    }
};

var golem2 = new Golem
{
    UserId = 2,
    PartIds = new []
    {
        new []{0, 1, 0},
        new []{0, 0, 0},
        new []{0, 0, 0},
    }
};

var parts = await client.GetParts(cancellationToken);

var partsCache = new PartsCache(parts);

var results = Resolver.GetOutcome(golem1, golem2, partsCache);

foreach (var result in results)
{
    Console.WriteLine(result);
}

var shop = new Shop(partsCache);
foreach (var part in shop.GetPartsForRound(0))
{
    view.PrintPart(part);
}

await client.CreateGolem(new CreateGolemRequest{Item = golem1}, cancellationToken);

var golems = await client.GetGolemSelection(new CancellationToken());

Console.WriteLine(golems[0].UserId);

var golemToUpdate = golems.First(g => g.Version == 1);

var updateRequest = new UpdateRequest
{
    TableName = "golem",
    Key = new
    {
        Timestamp = golemToUpdate.Timestamp,
        UserId = golemToUpdate.UserId
    },
    UpdateExpression = "set Version = :newVersion",
    ConditionExpression = "Version = :version",
    ExpressionAttributeValues = new Dictionary<string, object>
    {
        { ":newVersion", 0 },
        { ":version", 1 }
    }
};

var json = JsonSerializer.Serialize(updateRequest, new JsonSerializerOptions(new JsonSerializerOptions{PropertyNamingPolicy = null}));
await client.UpdateGolem(updateRequest, cancellationToken);