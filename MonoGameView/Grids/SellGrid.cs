using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameView.Grids;

public class SellGrid : Grid
{
    private readonly ShopView _shopView;

    public SellGrid(ShopView shopView, Texture2D blankTexture, Texture2D highlightTexture) : base(1, 1, blankTexture, highlightTexture)
    {
        _shopView = shopView;
    }

    protected override void UpdateSource()
    {
        var part = Sockets[0][0].StoredPart;
        if (part == null) return;
        _shopView.SellCluster(part.Parent, part.Part);
    }

    protected override Dictionary<DraggablePart, PartSocket> GetCandidates(Point mousePosition, DraggablePartCluster cluster, out Vector2 clusterPosition)
    {
        var candidatePairs = new Dictionary<DraggablePart, PartSocket>();
        clusterPosition = new Vector2();
        
        Vector2? socketUnderMouseCoords = null;
        for (var x = 0; x < Sockets.Length; x++)
        {
            for (var y = 0; y < Sockets[x].Length; y++)
            {
                if (Sockets[x][y].PointInBounds(mousePosition))
                {
                    socketUnderMouseCoords = new Vector2(x, y);
                }
            }
        }

        // If mouse not over a socket, all parts should be invalid.
        if (socketUnderMouseCoords == null)
        {
            return candidatePairs;
        }
        candidatePairs.Add(cluster.GetDraggableUnderMouse(mousePosition), Sockets[0][0]);
        return candidatePairs;
    }
}