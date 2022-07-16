using System;
using System.Collections.Generic;
using System.Linq;
using GolemCore.Validation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameView.Events;

namespace MonoGameView.Grids;

public abstract class Grid
{
    protected readonly PartSocket[][] Sockets;

    private readonly EventHandler<ClusterDraggedArgs> _onStartDrag;
    private readonly EventHandler<ClusterDraggedArgs> _onEndDrag;

    private DraggablePartCluster _currentCluster;

    protected Grid(int width, int height, Texture2D blankTexture, Texture2D highlightTexture)
    {
        Sockets = new PartSocket[width][];
        _onStartDrag = (sender, eventArgs) =>
        {
            _currentCluster = eventArgs.Cluster;
        };
        _onEndDrag = (sender, eventArgs) =>
        {
            UnsocketPartsOfCluster(_currentCluster);
            var clusterSocketed = SocketClusterAtMouse(eventArgs.MouseState, eventArgs.Cluster);
            _currentCluster = null;
            ClearHighlights();
            if (clusterSocketed)
            {
                eventArgs.ClusterSocketed();
            }
        };
        for(var x = 0; x < width; x++)
        {
            for(var y = 0; y < height; y++)
            {
                Sockets[x] ??= new PartSocket[height];
                Sockets[x][y] ??= new PartSocket(new Vector2(x, y), Constants.SocketSize, Constants.SocketSize, blankTexture, highlightTexture);
            }
        }
    }

    protected abstract void UpdateSource();

    public void Update(MouseState mouseState)
    {
        if (_currentCluster != null)
        {
            ClearHighlights();
            DisplayValidation(mouseState.Position, _currentCluster);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var line in Sockets)
        {
            foreach (var socket in line)
            {
                socket.Draw(spriteBatch);
            }
        }
    }

    public void SubscribeToClusterEvents(ClusterManager clusterManager)
    {
        clusterManager.StartDrag += _onStartDrag;
        clusterManager.EndDrag += _onEndDrag;
    }

    private void UnsocketPartsOfCluster(DraggablePartCluster cluster)
    {
        foreach (var line in Sockets)
        {
            foreach (var socket in line)
            {
                if (cluster.Contains(socket.StoredPart))
                {
                    socket.ClearStorage();
                }
            }
        }
        UpdateSource();
    }

    private bool SocketClusterAtMouse(MouseState mouseState, DraggablePartCluster cluster)
    {
        var candidatePairs = GetCandidates(mouseState.Position, cluster, out var clusterPosition);

        if (candidatePairs.Count > 0 && candidatePairs.All(kvp => kvp.Value != null))
        {
            foreach (var (part, socket) in candidatePairs)
            {
                socket.StorePart(part);
            }
            cluster.SetPosition(clusterPosition);
            UpdateSource();
            return true;
        }

        return false;
    }

    private void ClearHighlights()
    {
        foreach (var line in Sockets)
        {
            foreach (var socket in line)
            {
                socket.Highlight = false;
            }
        }
    }

    private void DisplayValidation(Point mouseStatePosition, DraggablePartCluster cluster)
    {
        var candidates = GetCandidates(mouseStatePosition, cluster, out _);

        foreach (var (part, socket) in candidates)
        {
            if (socket == null)
            {
                part.TempInvalid = true;
            }
            else
            {
                part.TempInvalid = false;
                socket.Highlight = true;
            }
        }
    }

    protected virtual Dictionary<DraggablePart, PartSocket> GetCandidates(Point mousePosition, DraggablePartCluster cluster, out Vector2 clusterPosition)
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

        var shapeCoords = cluster.MousePositionToPartCoords(mousePosition);
        
        for (var x = 0; x < Sockets.Length; x++)
        {
            for (var y = 0; y < Sockets[x].Length; y++)
            {
                var draggableToCheck = cluster.GetDraggableAtCoords(x, y);

                var offsetX = x + (int) socketUnderMouseCoords.Value.X - (int) shapeCoords.X;
                var offsetY = y + (int) socketUnderMouseCoords.Value.Y - (int) shapeCoords.Y;

                // If there is a draggablePart at these indices, and it's either out of bounds or the socket is occupied
                // then mark that part as invalid using a null value in the dictionary
                if (offsetX < 0 || offsetX >= Sockets.Length || offsetY < 0 || offsetY >= Sockets[x].Length ||
                    (draggableToCheck != null && Sockets[offsetX][offsetY].StoredPart != null))
                {
                    if (draggableToCheck != null)
                    {
                        candidatePairs.Add(draggableToCheck, null);
                    }
                }
                else
                {
                    if (x == 0 && y == 0) clusterPosition = Sockets[offsetX][offsetY].Position;
                    if (draggableToCheck != null)
                    {
                        candidatePairs.Add(draggableToCheck, Sockets[offsetX][offsetY]);
                    }
                }
            }
        }

        return candidatePairs;
    }
}