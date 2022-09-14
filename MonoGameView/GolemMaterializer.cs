using System.Collections.Generic;
using System.Linq;
using GolemCore;
using GolemCore.Extensions;
using GolemCore.Models.Golem;
using GolemCore.Models.Part;
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
        foreach (var id in golem.NonEmptyIdList.Distinct())
        {
            var cluster = _clusterManager.CreateCluster(_cache.Get(id.ToPartId()), 0, 0);
            clusters.Add(cluster);
            partIdToPartList.Add(id, cluster.GetDraggablePartList());
        }

        var parts = new DraggablePart[golem.PartIds.Length][];
        for (var i = 0; i < parts.Length; i++)
        {
            parts[i] = new[] {null, null, null, (DraggablePart) null};
        }
        
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
        
        grid.SetGolem(golem, parts);

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