using GolemCore.Extensions;
using GolemCore.Models.Enums;
using GolemCore.Models.Golem;
using GolemCore.Models.Part;

namespace GolemCore.Validation.PartRestrictions;

public abstract class PartRestriction
{
    private readonly PartsCache _partsCache;

    public PartRestriction(PartsCache partsCache)
    {
        _partsCache = partsCache;
    }
    
    public abstract List<ValidationProblem> GetProblems(string partIdWithSuffix, Golem golem);

    protected bool AnyNeighboursWithTag(string partIdWithSuffix, Golem golem, PartTag tag)
    {
        var neighbours = GetNeighboursIncludingDiagonals(partIdWithSuffix, golem);
        return neighbours.Any(n => n.Tags != null && n.Tags.Contains(tag));
    }
    
    protected bool OrthogonalNeighboursWithTag(string partIdWithSuffix, Golem golem, PartTag tag)
    {
        var neighbours = GetNeighboursExcludingDiagonals(partIdWithSuffix, golem);
        return neighbours.Any(n => n.Tags != null && n.Tags.Contains(tag));
    }

    protected List<Part> GetNeighboursIncludingDiagonals(string partIdWithSuffix, Golem golem)
    {
        var neighbours = new List<Part>();
        var ids = golem.PartIds;
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
                        if (ids[resultY][resultX] != partIdWithSuffix) continue;

                        var candidateNeighbourId = ids[y][x];
                        if (candidateNeighbourId != partIdWithSuffix && candidateNeighbourId != "-1") // don't add other parts from the same cluster
                        {
                            neighbours.Add(_partsCache.Get(candidateNeighbourId.ToPartId()));
                        }
                    }
                }
            }
        }

        return neighbours;
    }
    
    protected List<Part> GetNeighboursExcludingDiagonals(string partIdWithSuffix, Golem golem)
    {
        var neighbours = new List<Part>();
        var ids = golem.PartIds;
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
                        if (ids[resultY][resultX] != partIdWithSuffix) continue;

                        var candidateNeighbourId = ids[y][x];
                        if (candidateNeighbourId != partIdWithSuffix && candidateNeighbourId != "-1") // don't add other parts from the same cluster
                        {
                            neighbours.Add(_partsCache.Get(candidateNeighbourId.ToPartId()));
                        }
                    }
                }
            }
        }

        return neighbours;
    }
}