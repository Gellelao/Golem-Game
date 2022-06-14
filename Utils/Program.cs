using GolemCore.Models;
using GolemCore.Models.Enums;
using GolemCore.Models.Part;
using static Utils.PartsManager;

Console.WriteLine("Hello, World!");

await WritePartsFromDatabaseIntoFile("test.json");

var newParts = new List<Part>
{
    new()
    {
        Id = 0,
        Name = "Hand",
        Tier = Tier.Common,
        Tags = new[] {PartTag.Grabby},
        Stats = new List<Stat>
        {
            new()
            {
                Modifier = 1,
                Type = StatType.Attack
            },
            new()
            {
                Modifier = 1,
                Type = StatType.Health
            }
        },
        Shape = new[]
        {
            new[] {true, true, false, false},
            new[] {true, true, false, false},
            new[] {false, false, false, false},
            new[] {false, false, false, false}
        }
    },
    new()
    {
        Id = 1,
        Name = "Orb",
        Tier = Tier.Common,
        Tags = new[] {PartTag.Orb},
        Stats = new List<Stat>
        {
            new()
            {
                Modifier = 3,
                Type = StatType.Attack
            },
            new()
            {
                Modifier = 3,
                Type = StatType.Health
            }
        },
        Shape = new[]
        {
            new[] {true, false, false, false},
            new[] {false, false, false, false},
            new[] {false, false, false, false},
            new[] {false, false, false, false}
        }
    }
};

await PostAllParts(newParts);


// Updating without just PUTting a new itme with the same id:

// var updateRequest = new UpdateRequest
// {
//     TableName = "golem",
//     Key = new
//     {
//         Timestamp = golemToUpdate.Timestamp,
//         UserId = golemToUpdate.UserId
//     },
//     UpdateExpression = "set Version = :newVersion",
//     ConditionExpression = "Version = :version",
//     ExpressionAttributeValues = new Dictionary<string, object>
//     {
//         { ":newVersion", 0 },
//         { ":version", 1 }
//     }
// };
//
// var json = JsonSerializer.Serialize(updateRequest, new JsonSerializerOptions(new JsonSerializerOptions{PropertyNamingPolicy = null}));
// await client.UpdateGolem(updateRequest, cancellationToken);