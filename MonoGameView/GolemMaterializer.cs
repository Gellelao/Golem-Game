using System.Collections.Generic;
using System.Linq;
using GolemCore;
using GolemCore.Extensions;
using GolemCore.Models.Golem;
using GolemCore.Models.Part;
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
        var clusters = new Dictionary<string, List<DraggablePart>>();
        foreach (var id in golem.NonEmptyIdList.Distinct())
        {
            var cluster = _clusterManager.CreateCluster(_cache.Get(id.ToPartId()), 0, 0);
            clusters.Add(id, cluster.GetDraggablePartList());
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
                parts[x][y] = clusters[id].First();
                clusters[id].RemoveAt(0);
            }
        }
        
        grid.SetGolem(golem, parts);
    }
}