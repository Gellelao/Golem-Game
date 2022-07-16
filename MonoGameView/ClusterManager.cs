using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameView.Events;

namespace MonoGameView;

public class ClusterManager
{
    public event EventHandler<ClusterDraggedArgs> StartDrag;
    public event EventHandler<ClusterDraggedArgs> EndDrag;
    
    private readonly EventHandler<PartTransactionArgs> _onPartBought;
    private readonly EventHandler<PartTransactionArgs> _onPartSold;

    private readonly LinkedList<DraggablePartCluster> _clusters;
    private DraggablePartCluster _draggedCluster;
    private bool _rightMousePressed;

    public ClusterManager(Texture2D grayTexture, Texture2D redTexture, SpriteFont arialFont)
    {
        _clusters = new LinkedList<DraggablePartCluster>();
        
        _onPartBought = (sender, eventArgs) =>
        {
            AddCluster(new DraggablePartCluster(new Vector2(100, 100), grayTexture, arialFont, redTexture, eventArgs.Part));
        };
        _onPartSold = (sender, eventArgs) =>
        {
            RemoveCluster(eventArgs.Cluster);
        };
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
                
                _draggedCluster = cluster;
                
                MoveClusterToFront(cluster);
                cluster.SetInvalidOnAllParts(false);
                cluster.Grab(mouseState);
                
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

                if (!clusterDraggedArgs.ClusterWasSocketed)
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

    private void AddCluster(DraggablePartCluster cluster)
    {
        _clusters.AddFirst(cluster);
    }

    private void RemoveCluster(DraggablePartCluster cluster)
    {
        _clusters.Remove(cluster);
    }
    
    private void MoveClusterToFront(DraggablePartCluster cluster)
    {
        RemoveCluster(cluster);
        AddCluster(cluster);
    }

    public void SubscribeToShopEvents(ShopView shopView)
    {
        shopView.PartBought += _onPartBought;
        shopView.PartSold += _onPartSold;
    }
}