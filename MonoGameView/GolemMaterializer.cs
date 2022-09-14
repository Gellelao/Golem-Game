using System.Collections.Generic;
using System.Linq;
using GolemCore;
using GolemCore.Extensions;
using GolemCore.Models.Golem;
using Microsoft.Xna.Framework;
using MonoGameView.Grids;

namespace MonoGameView;

public class GolemMaterializer
{
    private readonly PartsCache _cache;
    private readonly ClusterManager _clusterManager;

    public GolemMaterializer(PartsCache cache, ClusterManager clusterManager)
    {
        _cache = cache;
        _clusterManager = clusterManager;
    }

    public void Materialize(Golem golem, GolemGrid grid)
    {
        var clusters = new List<DraggablePartCluster>();
        var partIdToPartList = new Dictionary<string, List<DraggablePart>>();
        
        // Create the clusters, making one for each unique partId on the golem. Multiple of the same part are
        // distinguished by a . suffix. Then, map the unique id to the list of parts created under that cluster
        foreach (var id in golem.NonEmptyIdList.Distinct())
        {
            var cluster = _clusterManager.CreateCluster(_cache.Get(id.ToPartId()), 0, 0);
            clusters.Add(cluster);
            partIdToPartList.Add(id, cluster.GetDraggablePartList());
        }

        // Initialize the 2d array that we're going to pass to the grid
        var parts = new DraggablePart[golem.PartIds.Length][];
        for (var i = 0; i < parts.Length; i++)
        {
            parts[i] = new[] {null, null, null, (DraggablePart) null};
        }
        
        // Fill in that parts array with the DraggableParts generated earlier. Looks up the draggablePart list
        // via the id in the Golem partIds, and remove the part from the list after use so that the next part
        // will be used on next loop
        for (var y = 0; y < golem.PartIds.Length; y++)
        {
            for (var x = 0; x < golem.PartIds.Length; x++)
            {
                var id = golem.PartIds[x][y];
                if (id == "-1") continue;
                parts[x][y] = partIdToPartList[id].First();
                partIdToPartList[id].RemoveAt(0);
            }
        }
        
        // Pass the parts to the grid, which takes care of socketing them.
        // Part of the socketing process updates the DraggablePart postions
        grid.SetGolem(golem, parts);

        // Now that the draggableParts are in place, update the cluster position to match
        // Take the min x and y of each part to find the cluster pos (top left of square)
        foreach (var cluster in clusters)
        {
            var minX = float.MaxValue;
            var minY = float.MaxValue;
            foreach (var part in cluster.GetDraggablePartList())
            {
                if (part.Position.X < minX) minX = part.Position.X;
                if (part.Position.Y < minY) minY = part.Position.Y;
            }
            cluster.SetPosition(new Vector2(minX, minY));
        }
    }
}