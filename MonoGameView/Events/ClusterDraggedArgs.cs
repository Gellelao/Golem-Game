using System;
using Microsoft.Xna.Framework.Input;
using MonoGameView.Grids;

namespace MonoGameView.Events;

public class ClusterDraggedArgs : EventArgs
{
    public DraggablePartCluster Cluster{ get; }
    public MouseState MouseState { get; }
    
    public Grid GridClusterWasSocketedTo { get; private set; }

    public ClusterDraggedArgs(DraggablePartCluster cluster, MouseState mouseState)
    {
        Cluster = cluster;
        MouseState = mouseState;
        GridClusterWasSocketedTo = null;
    }

    public void ClusterSocketed(Grid receiver)
    {
        GridClusterWasSocketedTo = receiver;
    }
}