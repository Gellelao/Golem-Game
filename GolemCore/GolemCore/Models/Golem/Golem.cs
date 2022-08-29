using System.Diagnostics.CodeAnalysis;
using System.Text;
using GolemCore.Extensions;
using GolemCore.Models.Enums;
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
        new []{"-1","-1","-1","-1"},
        new []{"-1","-1","-1","-1"},
        new []{"-1","-1","-1","-1"},
        new []{"-1","-1","-1","-1"}
    };

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
    
    /// <summary>
    /// Given a full part id, get the ids of parts that share an edge with that part, on this golem
    /// </summary>
    /// <param name="fullPartId">The part that marks the center of the neighbourhood search</param>
    /// <returns> A list of the ids of the found neighbours</returns>
    public List<string> GetOrthogonalNeighbourIds(string fullPartId)
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
                        // Don't consider the corners
                        if (offsetX == -1 && offsetY == -1) continue;
                        if (offsetX == -1 && offsetY == 1) continue;
                        if (offsetX == 1 && offsetY == -1) continue;
                        if (offsetX == 1 && offsetY == 1) continue;
                        
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
    
    /// <summary>
    /// Given a full part id, get the ids of parts that share an edge or corner with that part, on this golem
    /// </summary>
    /// <param name="fullPartId">The part that marks the center of the neighbourhood search</param>
    /// <returns> A list of the ids of the found neighbours</returns>
    public List<string> GetOrthodiagonalNeighbourIds(string fullPartId)
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
    
    public List<string> GetPartsNorthOf(string fullPartId)
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
    
    public List<Part.Part> GetActivatedParts(TurnStatus turnStatus, PartsCache partsCache)
    {
        var activatedParts = new List<Part.Part>();
        foreach (var partId in NonEmptyIdList)
        {
            var part = partsCache.Get(partId.ToPartId());
            foreach (var trigger in part.Triggers)
            {
                if (!trigger.Triggered(turnStatus)) continue;
                var neighbouringPartIds = trigger.EffectRange.IdsOfNeighbouringParts(partId, this);
                var neighbouringParts = neighbouringPartIds.Select(id => partsCache.Get(id.ToPartId()));
                var eligibleParts = neighbouringParts.Where(np => trigger.WouldActivate(np));
                activatedParts.AddRange(eligibleParts.Where(p => p.Effects.Any()));
            }
        }
        return activatedParts.Distinct().ToList();
    }
}