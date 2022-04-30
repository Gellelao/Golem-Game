using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

    private void UpdateGolem()
    {
        // foreach (var socket in _sockets)
        // {
        //     // TODO: Why do X and Y need to be flipped
        //     _golem.PartIds[(int)socket.GolemPartIndex.Y][(int)socket.GolemPartIndex.X] = socket.StoredPart == null ? "-1" : socket.StoredPart.Part.Id.ToString();
        // }
        // Console.WriteLine(_golem);
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
        // foreach (var socket in _sockets)
        // {
        //     if (socket.StoredPart != null) continue;
        //     if (!socket.PointInBounds(mouseState.Position)) continue;
        //             
        //     socket.StorePart(part);
        //     UpdateGolem();
        //     break;
        // }
        Console.WriteLine("======================");

        Vector2? socketUnderMouseCoords = null;
        for (var x = 0; x < _sockets.Length; x++)
        {
            for (var y = 0; y < _sockets[x].Length; y++)
            {
                if (_sockets[x][y].PointInBounds(mouseState.Position))
                {
                    socketUnderMouseCoords = new Vector2(x, y);
                    Console.WriteLine($"socket under mouse coords at {x},{y}");
                }
            }
        }

        // If mouse not over a socket, nothing to do here
        if (socketUnderMouseCoords == null)
        {
            Console.WriteLine("Mouse not over socket, returning");
            return;
        }

        var partUnderMouse = cluster.GetDraggableUnderMouse(mouseState.Position);
        var shapeCoords = cluster.GetCoordsForPart(partUnderMouse);
        Console.WriteLine($"Shape coords: {shapeCoords.X},{shapeCoords.Y}");

        Dictionary<DraggablePart, PartSocket> candidatePairs = new Dictionary<DraggablePart, PartSocket>();
        for (var x = 0; x < _sockets.Length; x++)
        {
            for (var y = 0; y < _sockets[x].Length; y++)
            {
                var draggableToCheck = cluster.GetDraggableAtCoords(x, y);
                Console.WriteLine($"checking draggable at {x},{y}");
                if (draggableToCheck != null)
                {
                    var offsetX = x + (int) socketUnderMouseCoords.Value.X - (int) shapeCoords.X;
                    var offsetY = y + (int) socketUnderMouseCoords.Value.Y - (int) shapeCoords.Y;

                    if (x == 0 && y == 0) cluster.SetPosition(_sockets[offsetX][offsetY].Position);
                    
                    // This piece doesn't fit, so indicate that with a null key
                    if (offsetX < 0 || offsetX >= _sockets[x].Length || offsetY < 0 || offsetY >= _sockets.Length)
                    {
                        Console.WriteLine($"piece does not fit (offsetX {offsetX}, offsetY {offsetY})");
                        candidatePairs.Add(draggableToCheck, null);
                    }
                    else
                    {
                        Console.WriteLine($"piece fits! (offsetX {offsetX}, offsetY {offsetY})");
                        candidatePairs.Add(draggableToCheck, _sockets[offsetX][offsetY]);
                    }
                }
            }
        }

        if (candidatePairs.All(kvp => kvp.Value != null))
        {
            foreach (var kvp in candidatePairs)
            {
                kvp.Value.StorePart(kvp.Key);
            }
        }
        else
        {
            throw new NoNullAllowedException("A part was outside bounds");
        }

    }

    public bool HighlightCandidateSockets(Point mousePosition)
    {
        return false;
        // var socket = GetSocketUnderMouse(mousePosition);
        // if (socket == null || socket.StoredPart != null) return false;
        // socket.Highlight = true;
        // return true;
    }

    public void ClearHighlights()
    {
       // _sockets.ForEach(s => s.Highlight = false);
    }

    private PartSocket GetSocketUnderMouse(Point mousePosition)
    {
        return null;
        //return _sockets.FirstOrDefault(socket => socket.PointInBounds(mousePosition));
    }
}