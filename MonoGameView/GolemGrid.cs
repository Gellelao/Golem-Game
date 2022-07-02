using System;
using System.Collections.Generic;
using System.Linq;
using GolemCore.Extensions;
using GolemCore.Models.Golem;
using GolemCore.Validation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView;

public class GolemGrid
{
    private Golem _golem;
    private readonly PartValidator _validator;
    private PartSocket[][] _sockets;

    public GolemGrid(Golem golem, PartValidator validator, Texture2D blankTexture, Texture2D highlightTexture)
    {
        _golem = golem;
        _validator = validator;
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
        var partIdToCluster = new Dictionary<int, List<DraggablePartCluster>>();
        var valid = true;
        
        foreach (var line in _sockets)
        {
            foreach (var socket in line)
            {
                if (socket.StoredPart != null)
                {
                    var partId = socket.StoredPart.Part.Id;
                    var parentCluster = socket.StoredPart.Parent;
                    
                    // Get the suffix (e.g 1.2, with 2 being the suffix and 1 being the part id) by adding each
                    // unique cluster to a list, and getting the index of that cluster in the list
                    var suffix = 0;
                    if (partIdToCluster.ContainsKey(partId))
                    {
                        if (!partIdToCluster[partId].Contains(parentCluster))
                        {
                            partIdToCluster[partId].Add(parentCluster);
                        }

                        suffix = partIdToCluster[partId].IndexOf(parentCluster);
                    }
                    else
                    {
                        partIdToCluster.Add(partId, new List<DraggablePartCluster>{ parentCluster });
                    }

                    var idWithSuffix = socket.StoredPart.Part.Id + (suffix > 0 ? $".{suffix}" : "");
                    _golem.PartIds[(int) socket.GolemPartIndex.Y][(int) socket.GolemPartIndex.X] = idWithSuffix;
                }
                else
                {
                    _golem.PartIds[(int) socket.GolemPartIndex.Y][(int) socket.GolemPartIndex.X] = "-1";   
                }
            }
        }
        
        foreach (var line in _golem.PartIds)
        {
            foreach (var fullId in line.Where(id => id != "-1"))
            {
                var idTokens = fullId.Split('.');
                var partId = fullId.ToPartId();
                DraggablePartCluster cluster;
                if (idTokens.Length > 1)
                {
                    var suffix = int.Parse(idTokens[1]);
                    cluster = partIdToCluster[partId][suffix];
                }
                else
                {
                    cluster = partIdToCluster[partId].First();
                }
                
                var validationProblems = _validator.Validate(fullId, _golem);

                if (validationProblems.Any())
                {
                    valid = false;
                    foreach (var problem in validationProblems)
                    {
                        Console.WriteLine(problem.Reason);
                    }
                    cluster.SetInvalidOnAllParts(true);
                }
                else
                {
                    cluster.SetInvalidOnAllParts(false);
                }
            }
        }

        if (valid)
        {
            // only allow the golem to be uploaded if it is valid. But locally we permit an invalid golem
        }
        Console.WriteLine(_golem);
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
        UpdateGolem();
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
            UpdateGolem();
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

        if (!candidates.Any())
        {
            cluster.ClearTempInvalids();
        }
        
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
}