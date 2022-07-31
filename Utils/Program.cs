using GolemCore.Models;
using GolemCore.Models.Enums;
using GolemCore.Models.Part;
using static Utils.PartsManager;

Console.WriteLine("Hello, World!");

var newParts = new List<Part>
{
    new()
    {
        Id = 0,
        Name = "Hand",
        Tier = Tier.Common,
        Tags = new List<PartTag>{PartTag.Grabby},
        Stats = new List<Stat>
        {
            new()
            {
                Modifier = 1,
                Type = StatType.Attack,
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
        Restrictions = new List<NeighbourhoodRequirement>
        {
            new()
            {
                Locator = Locator.Orthogonal,
                Requirement = Requirement.OneOrMore,
                Tag = PartTag.Grabby
            }
        }
    },
    new()
    {
        Id = 2,
        Name = "Ember",
        Tier = Tier.Common,
        Tags = new List<PartTag>{PartTag.Fire},
        Stats = new List<Stat>
        {
            new()
            {
                Modifier = 1,
                Type = StatType.Attack,
                StatMultiplier = new Neighbourhood
                {
                    Locator = Locator.Orthodiagonal,
                    Tag = PartTag.Fire
                }
            },
        }
    },
    new()
    {
        Id = 3,
        Name = "Stone",
        Tier = Tier.Common,
        Stats = new List<Stat>
        {
            new()
            {
                Modifier = 3,
                Type = StatType.Health,
                Condition = new NeighbourhoodRequirement
                {
                    Locator = Locator.Orthodiagonal,
                    Requirement = Requirement.OneOrMore,
                    Tag = PartTag.Mossy
                }
            },
        },
        Shape = new[]
        {
            new[] {true, false, false, false},
            new[] {true, false, false, false},
            new[] {true, false, false, false},
            new[] {true, false, false, false}
        }
    },
    new()
    {
        Id = 4,
        Name = "Mossball",
        Tier = Tier.Common,
        Tags = new List<PartTag>{PartTag.Mossy},
        Stats = new List<Stat>
        {
            new()
            {
                Modifier = 1,
                Type = StatType.Health
            },
        },
        Shape = new[]
        {
            new[] {true, false, false, false},
            new[] {false, false, false, false},
            new[] {false, false, false, false},
            new[] {false, false, false, false}
        }
    },
    new()
    {
        Id = 5,
        Name = "Rain Dish",
        Tier = Tier.Common,
        Tags = new List<PartTag>(),
        Stats = new List<Stat>
        {
            new()
            {
                Modifier = 3,
                Type = StatType.Water
            },
        },
        Shape = new[]
        {
            new[] {true, true, false, false},
            new[] {false, true, false, false},
            new[] {false, false, false, false},
            new[] {false, false, false, false}
        },
        Restrictions = new List<NeighbourhoodRequirement>
        {
            new()
            {
                Locator = Locator.AllNorthernParts,
                Requirement = Requirement.Zero,
                Tag = PartTag.NotNull
            }
        }
    }
};

await PostAllParts(newParts);

await WritePartsFromDatabaseIntoFile("test.json");

Console.WriteLine("Done");

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