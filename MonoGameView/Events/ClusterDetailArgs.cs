using System;
using GolemCore.Models.Part;

namespace MonoGameView.Events;

public class ClusterDetailArgs : EventArgs
{
    public DraggablePartCluster Cluster { get; }
    
    public ClusterDetailArgs(DraggablePartCluster cluster)
    {
        Cluster = cluster;
    }
}