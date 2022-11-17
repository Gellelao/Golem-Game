using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;
using GolemCore.Extensions;
using GolemCore.Models.Enums;
using GolemCore.Models.Triggers;
using GolemCore.Resolver;

namespace GolemCore.Models.Golem;

public class Golem
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public DateTime Timestamp { get; init; } = DateTime.Now;

    public int UserId { get; init; }
    public int Version { get; init; } = 1;
    public string[][] PartIds { get; init; } =
    {
        new []{"-1","-1","-1","-1","-1","-1"},
        new []{"-1","-1","-1","-1","-1","-1"},
        new []{"-1","-1","-1","-1","-1","-1"},
        new []{"-1","-1","-1","-1","-1","-1"},
        new []{"-1","-1","-1","-1","-1","-1"},
        new []{"-1","-1","-1","-1","-1","-1"}
    };

    [JsonIgnore]
    public List<string> NonEmptyIdList
    {
        get { return PartIds.SelectMany(p => p).Where(id => id != "-1").ToList(); }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"User {UserId}");
        foreach (var line in PartIds)
        {
            builder.AppendJoin(", ", line);
            builder.AppendLine();
        }

        return builder.ToString();
    }
    
    public List<Part.Part> GetPartsActivatedByTurn(int turnNumber, PartsCache partsCache)
    {
        var activatedParts = new List<Part.Part>();
        foreach (var partId in NonEmptyIdList)
        {
            var part = partsCache.Get(partId.ToPartId());
            foreach (var trigger in part.Triggers)
            {
                if (trigger is not TurnTrigger turnTrigger) continue;
                if (!turnTrigger.Triggered(turnNumber)) continue;
                activatedParts.AddRange(GetActivatedParts(trigger, partId, partsCache));
            }
        }
        return activatedParts.Distinct().ToList();
    }

    public List<Part.Part> GetPartsActivatedByStatChange(StatChange statChange, Target target, PartsCache partsCache)
    {
        var activatedParts = new List<Part.Part>();
        foreach (var partId in NonEmptyIdList)
        {
            var part = partsCache.Get(partId.ToPartId());
            foreach (var trigger in part.Triggers)
            {
                if (trigger is not StatChangeTrigger statTrigger) continue;
                if (statTrigger.Triggered(statChange, target))
                {
                    activatedParts.AddRange(GetActivatedParts(trigger, partId, partsCache));
                }
            }
        }
        return activatedParts.Distinct().ToList();
    }

    public List<string> GetNeighbourIds(string fullPartId, Locator locator)
    {
        var neighbours = new List<string>();
        switch (locator)
        {
            case Locator.Self:
                neighbours.Add(fullPartId);
                break;
            case Locator.AllNorthernParts:
                neighbours.AddRange(GetPartsNorthOf(fullPartId));
                break;
            case Locator.Orthogonal:
            case Locator.Diagonal:
            case Locator.Orthodiagonal:
            default:
                neighbours.AddRange(GetSurroundingNeighbourIds(fullPartId, locator));
                break;
        }

        return neighbours;
    }
    
    /// <summary>
    /// Given a full part id, get the ids of parts that share an edge with that part, on this golem
    /// </summary>
    /// <param name="fullPartId">The part that marks the center of the neighbourhood search</param>
    /// <returns> A list of the ids of the found neighbours</returns>
    private List<string> GetSurroundingNeighbourIds(string fullPartId, Locator locator)
    {
        var neighbours = new List<string>();
        var ids = PartIds;
        for (var y = 0; y < ids.Length; y++)
        {
            for (var x = 0; x < ids[y].Length; x++)
            {
                for (var offsetY = -1; offsetY < 2; offsetY++)
                {
                    for (var offsetX = -1; offsetX < 2; offsetX++)
                    {
                        if (locator == Locator.Orthogonal)
                        {
                            // Don't consider the corners
                            if (offsetX == -1 && offsetY == -1) continue;
                            if (offsetX == -1 && offsetY == 1) continue;
                            if (offsetX == 1 && offsetY == -1) continue;
                            if (offsetX == 1 && offsetY == 1) continue;
                        }
                        if (locator == Locator.Diagonal)
                        {
                            // Only consider the corners
                            if (offsetX == 0 && offsetY == -1) continue;
                            if (offsetX == 0 && offsetY == 1) continue;
                            if (offsetX == 1 && offsetY == 0) continue;
                            if (offsetX == -1 && offsetY == 0) continue;
                        }
                        var resultY = y + offsetY;
                        var resultX = x + offsetX;
                        if (resultY < 0 || resultY >= ids.Length || resultX < 0 || resultX >= ids[y].Length) continue;
                        if (ids[resultY][resultX] != fullPartId) continue;

                        var candidateNeighbourId = ids[y][x];
                        if (candidateNeighbourId != fullPartId && candidateNeighbourId != "-1") // don't add other parts from the same cluster
                        {
                            neighbours.Add(candidateNeighbourId);
                        }
                    }
                }
            }
        }

        return neighbours;
    }

    private List<string> GetPartsNorthOf(string fullPartId)
    {
        var neighbours = new List<string>();
        var ids = PartIds;
        for (var y = 0; y < ids.Length; y++)
        {
            for (var x = 0; x < ids[y].Length; x++)
            {
                if (ids[y][x] != fullPartId) continue;
                for (var i = y; i >= 0; i--)
                {
                    if (ids[i][x] != "-1" && ids[i][x] != fullPartId)
                    {
                        neighbours.Add(ids[i][x]);
                    }
                }
            }
        }

        return neighbours;
    }

    private List<Part.Part> GetActivatedParts(Trigger trigger, string partId, PartsCache cache)
    {
        var neighbouringPartIds = GetNeighbourIds(partId, trigger.EffectRange);
        var neighbouringParts = neighbouringPartIds.Select(id => cache.Get(id.ToPartId()));
        var eligibleParts = neighbouringParts.Where(trigger.WouldActivate);
        return eligibleParts.Where(p => p.Effects.Any()).ToList();
    }
}