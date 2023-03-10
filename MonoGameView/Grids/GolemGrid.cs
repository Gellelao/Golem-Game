using System;
using System.Collections.Generic;
using System.Linq;
using GolemCore.Extensions;
using GolemCore.Models.Golem;
using GolemCore.Validation;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameView.Grids;

public class GolemGrid : Grid
{
    public bool Valid { get; private set; }
    protected readonly PartValidator Validator;
    private Golem _golem;

    public GolemGrid(Golem golem, PartValidator validator, Texture2D blankTexture, Texture2D highlightTexture) : base(golem.PartIds.Length, golem.PartIds[0].Length, blankTexture, highlightTexture)
    {
        Validator = validator;
        _golem = golem;
    }

    protected override void UpdateSource(bool doValidation)
    {
        var partIdToCluster = new Dictionary<int, List<DraggablePartCluster>>();
        
        foreach (var line in Sockets)
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
                    _golem.PartIds[(int) socket.GolemPartIndex.X][(int) socket.GolemPartIndex.Y] = idWithSuffix;
                }
                else
                {
                    _golem.PartIds[(int) socket.GolemPartIndex.X][(int) socket.GolemPartIndex.Y] = "-1";   
                }
            }
        }

        if (doValidation)
        {
            ValidateGolem(partIdToCluster);
        }
    }

    public void SetGolem(Golem golem, DraggablePart[][] draggableParts)
    {
        _golem = golem;
        for (var x = 0; x < Sockets.Length; x++)
        {
            for (var y = 0; y < Sockets[x].Length; y++)
            {
                if (draggableParts[x][y] == null) continue;
                Sockets[x][y].StorePart(draggableParts[x][y]);
            }
        }

        // Only valid golems can be uploaded
        // CURRENTLY the only thing calling this is the Summon() button, but if that changes might need to rethink this
        Valid = true;
    }

    private void ValidateGolem(Dictionary<int, List<DraggablePartCluster>> partIdToCluster)
    {
        var valid = true;
        
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
                
                var validationProblems = Validator.Validate(fullId, _golem);

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

        Valid = valid;
    }
}