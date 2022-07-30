using GolemCore.Models.Golem;

namespace GolemCore.Extensions;

public static class GolemExtensions
{
    /// <summary>
    /// Given a full part id, get the ids of parts that share an edge with that part, on this golem
    /// </summary>
    /// <param name="golem">The golem to search</param>
    /// <param name="fullPartId">The part that marks the center of the neighbourhood search</param>
    /// <returns> A list of the ids of the found neighbours</returns>
    public static List<string> GetOrthogonalNeighbourIds(this Golem golem, string fullPartId)
    {
        var neighbours = new List<string>();
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
    /// <param name="golem">The golem to search</param>
    /// <param name="fullPartId">The part that marks the center of the neighbourhood search</param>
    /// <returns> A list of the ids of the found neighbours</returns>
    public static List<string> GetOrthodiagonalNeighbourIds(this Golem golem, string fullPartId)
    {
        var neighbours = new List<string>();
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
}