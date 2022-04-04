using GolemCore;
using GolemCore.Models;

Console.WriteLine("Hello, World!");

var client = GolemApiClientFactory.Create();

var cancellationToken = new CancellationToken();

var golem1 = new Golem
{
    UserId = "1",
    PartIds = new []
    {
        new []{1, 1, 1},
        new []{0, 0, 0},
        new []{1, 0, 1},
    }
};

var golem2 = new Golem
{
    UserId = "2",
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

foreach (var part in parts)
{
    Console.WriteLine(part.Name);
}

// await client.CreateGolem(new CreateGolemRequest{Item = golem1}, cancellationToken);
//
// var golems = await client.GetGolemSelection(new CancellationToken());
//
// golems.ForEach(g => Console.WriteLine(g.Parts[0][0]));
