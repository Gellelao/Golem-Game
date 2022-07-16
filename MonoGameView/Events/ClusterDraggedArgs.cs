using System;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView.Events;

public class ClusterDraggedArgs : EventArgs
{
    public DraggablePartCluster Cluster{ get; }
    public MouseState MouseState { get; }
    
    public bool ClusterWasSocketed { get; private set; }

    public ClusterDraggedArgs(DraggablePartCluster cluster, MouseState mouseState)
    {
        Cluster = cluster;
        MouseState = mouseState;
        ClusterWasSocketed = false;
    }

    public void ClusterSocketed()
    {
        ClusterWasSocketed = true;
    }
}