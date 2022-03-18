using GolemCore;
using GolemCore.Models;

Console.WriteLine("Hello, World!");
const string apiBase = "***REMOVED***/";

var client = GolemApiClientFactory.Create(apiBase, "***REMOVED***");

var cancellationToken = new CancellationToken();

var golem = new Golem
{
    Class = "ice",
    Id = 4,
    Size = "colossal"
};

await client.CreateGolem(new CreateGolemRequest{Item = golem}, cancellationToken);

var golems = await client.GetGolemSelection(new CancellationToken());

golems.ForEach(g => Console.WriteLine(g.Class));
