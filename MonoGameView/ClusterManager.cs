using System;
using System.Collections.Generic;
using System.Linq;
using GolemCore.Models.Part;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameView.Events;
using MonoGameView.Grids;

namespace MonoGameView;

public class ClusterManager
{
    private readonly Texture2D _grayTexture;
    private readonly Texture2D _redTexture;
    private readonly SpriteFont _arialFont;
    public event EventHandler<ClusterDraggedArgs> StartDrag;
    public event EventHandler<ClusterDraggedArgs> EndDrag;
    public event EventHandler<ClusterDetailArgs> ClusterSocketed;

    private readonly LinkedList<DraggablePartCluster> _clusters;
    private DraggablePartCluster _draggedCluster;
    private bool _rightMousePressed;

    public ClusterManager(Texture2D grayTexture, Texture2D redTexture, SpriteFont arialFont)
    {
        _grayTexture = grayTexture;
        _redTexture = redTexture;
        _arialFont = arialFont;
        _clusters = new LinkedList<DraggablePartCluster>();
    }

    public void DrawClusters(SpriteBatch spriteBatch)
    {
        foreach (var cluster in _clusters.Reverse())
        {
            cluster.Draw(spriteBatch);
        }
    }

    public void Update(MouseState mouseState)
    {
        if (_draggedCluster != null)
        {
            _draggedCluster.ClearTempInvalids();
            
            // Rotate
            if (_rightMousePressed && mouseState.RightButton == ButtonState.Released)
            {
                _draggedCluster.Rotate();
            }

            _rightMousePressed = mouseState.RightButton == ButtonState.Pressed;
        }
        
        // New drag
        if (_draggedCluster == null && mouseState.LeftButton == ButtonState.Pressed)
        {
            foreach (var cluster in _clusters)
            {
                if (cluster.GetDraggableUnderMouse(mouseState.Position) == null) continue;
                
                var grabbed = cluster.Grab(mouseState);
                if (!grabbed) continue;
                
                _draggedCluster = cluster;
                
                MoveClusterToFront(cluster);
                cluster.SetInvalidOnAllParts(false);
                
                StartDrag?.Invoke(this, new ClusterDraggedArgs(_draggedCluster, mouseState));
                
                break;
            }
        }
        
        // Stop drag
        if (mouseState.LeftButton == ButtonState.Released)
        {
            if (_draggedCluster != null)
            {
                _draggedCluster.Release();

                var clusterDraggedArgs = new ClusterDraggedArgs(_draggedCluster, mouseState);
                EndDrag?.Invoke(this, clusterDraggedArgs);

                if (clusterDraggedArgs.GridClusterWasSocketedTo != null && clusterDraggedArgs.GridClusterWasSocketedTo.GetType() != typeof(SellGrid))
                {
                    ClusterSocketed?.Invoke(this, new ClusterDetailArgs(_draggedCluster));
                }
                else
                {
                    _draggedCluster.RevertToPositionBeforeDrag();
                }
                _draggedCluster.ClearTempInvalids();
                _draggedCluster = null;
            }
        }
        
        // Finally, update the clusters
        foreach (var cluster in _clusters)
        {
            cluster.Update(mouseState);
        }
    }

    public DraggablePartCluster CreateCluster(Part part, int x, int y)
    {
        var cluster = new DraggablePartCluster(new Vector2(x, y), _grayTexture, _arialFont, _redTexture, part);
        AddCluster(cluster);
        return cluster;
    }

    private void AddCluster(DraggablePartCluster cluster)
    {
        _clusters.AddFirst(cluster);
    }

    public void RemoveCluster(DraggablePartCluster cluster)
    {
        _clusters.Remove(cluster);
    }
    
    private void MoveClusterToFront(DraggablePartCluster cluster)
    {
        RemoveCluster(cluster);
        AddCluster(cluster);
    }

    public void RemoveAllClusters(List<DraggablePartCluster> clustersToRemove)
    {
        foreach (var cluster in clustersToRemove)
        {
            _clusters.Remove(cluster);
        }
    }
}