using System.Collections.Generic;
using System.Linq;
using GolemCore;
using GolemCore.Extensions;
using GolemCore.Models.Golem;
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
        foreach (var id in golem.NonEmptyIdList.Distinct())
        {
            clusters.Add(_clusterManager.CreateCluster(_cache.Get(id.ToPartId()), 0, 0));
        }
            
        grid.SetGolem();
    }
}