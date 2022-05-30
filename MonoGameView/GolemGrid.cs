using System;
using System.Collections.Generic;
using System.Linq;
using GolemCore.Models;
using GolemCore.Models.Golem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView;

public class GolemGrid
{
    private Golem _golem;
    private PartSocket[][] _sockets;

    public GolemGrid(Golem golem, Texture2D blankTexture, Texture2D highlightTexture)
    {
        _golem = golem;
        _sockets = new PartSocket[golem.PartIds.Length][];
        for(var x = 0; x < _golem.PartIds.Length; x++)
        {
            for(var y = 0; y < _golem.PartIds[x].Length; y++)
            {
                _sockets[y] ??= new PartSocket[golem.PartIds[y].Length];
                _sockets[x][y] = new PartSocket(new Vector2(x, y), Constants.SocketSize, Constants.SocketSize, blankTexture, highlightTexture);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var line in _sockets)
        {
            foreach (var socket in line)
            {
                socket.Draw(spriteBatch);
            }
        }
    }

    private void AddClusterToGolem(DraggablePartCluster cluster)
    {
        var highestExistingCount = _golem.GetHighestExistingCount(cluster.GetIdOfPartsInCluster());
        var suffix = highestExistingCount == 0 ? "" : $".{highestExistingCount}";

        foreach (var line in _sockets)
        {
            foreach (var socket in line)
            {
                _golem.PartIds[(int)socket.GolemPartIndex.Y][(int)socket.GolemPartIndex.X] = socket.StoredPart == null ? "-1" : socket.StoredPart.Part.Id + suffix;
            }
        }
        Console.WriteLine(_golem);
    }

    private bool GolemContainsAnyPartsWithId(int getIdOfPartsInCluster)
    {
        throw new NotImplementedException();
    }

    public void UnsocketPartsOfCluster(DraggablePartCluster cluster)
    {
        foreach (var line in _sockets)
        {
            foreach (var socket in line)
            {
                if (cluster.Contains(socket.StoredPart))
                {
                    socket.ClearStorage();
                }
            }
        }
    }

    public void SocketClusterAtMouse(MouseState mouseState, DraggablePartCluster cluster)
    {
        var candidatePairs = GetCandidates(mouseState.Position, cluster, out var clusterPosition);

        if (candidatePairs.Count > 0 && candidatePairs.All(kvp => kvp.Value != null))
        {
            foreach (var (part, socket) in candidatePairs)
            {
                socket.StorePart(part);
            }
            cluster.SetPosition(clusterPosition);
            AddClusterToGolem(cluster);
        }
    }

    private Dictionary<DraggablePart, PartSocket> GetCandidates(Point mousePosition, DraggablePartCluster cluster, out Vector2 clusterPosition)
    {
        var candidatePairs = new Dictionary<DraggablePart, PartSocket>();
        clusterPosition = new Vector2();
        
        Vector2? socketUnderMouseCoords = null;
        for (var x = 0; x < _sockets.Length; x++)
        {
            for (var y = 0; y < _sockets[x].Length; y++)
            {
                if (_sockets[x][y].PointInBounds(mousePosition))
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
        
        for (var x = 0; x < _sockets.Length; x++)
        {
            for (var y = 0; y < _sockets[x].Length; y++)
            {
                var draggableToCheck = cluster.GetDraggableAtCoords(x, y);

                var offsetX = x + (int) socketUnderMouseCoords.Value.X - (int) shapeCoords.X;
                var offsetY = y + (int) socketUnderMouseCoords.Value.Y - (int) shapeCoords.Y;

                // If there is a draggablePart at these indices, and it's either out of bounds or the socked it occupied
                // then mark that part as invalid using a null value in the dictionary
                if (offsetX < 0 || offsetX >= _sockets[x].Length || offsetY < 0 || offsetY >= _sockets.Length ||
                    (draggableToCheck != null && _sockets[offsetX][offsetY].StoredPart != null))
                {
                    if (draggableToCheck != null)
                    {
                        candidatePairs.Add(draggableToCheck, null);
                    }
                }
                else
                {
                    if (x == 0 && y == 0) clusterPosition = _sockets[offsetX][offsetY].Position;
                    if (draggableToCheck != null)
                    {
                        candidatePairs.Add(draggableToCheck, _sockets[offsetX][offsetY]);
                    }
                }
            }
        }

        return candidatePairs;
    }

    public void ClearHighlights()
    {
        foreach (var line in _sockets)
        {
            foreach (var socket in line)
            {
                socket.Highlight = false;
            }
        }
    }

    public void DisplayValidation(Point mouseStatePosition, DraggablePartCluster cluster)
    {
        var candidates = GetCandidates(mouseStatePosition, cluster, out _);
        foreach (var (part, socket) in candidates)
        {
            if (socket == null)
            {
                part.Invalid = true;
            }
            else
            {
                part.Invalid = false;
                socket.Highlight = true;
            }
        }
    }
}