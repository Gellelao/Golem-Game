using Microsoft.Xna.Framework.Input;

namespace MonoGameView.Events;

public class ClusterDraggedArgs
{
    public DraggablePartCluster Cluster{ get; }
    public MouseState MouseState { get; }

    public ClusterDraggedArgs(DraggablePartCluster cluster, MouseState mouseState)
    {
        Cluster = cluster;
        MouseState = mouseState;
    }
}