﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameView.Events;

namespace MonoGameView;

public class ClusterManager
{
    public event EventHandler<ClusterDraggedArgs> StartDrag;
    public event EventHandler<ClusterDraggedArgs> EndDrag;

    private readonly LinkedList<DraggablePartCluster> _clusters;
    private DraggablePartCluster _draggedCluster;
    private bool _rightMousePressed;

    public ClusterManager()
    {
        _clusters = new LinkedList<DraggablePartCluster>();
    }

    public void AddCluster(DraggablePartCluster cluster)
    {
        _clusters.AddFirst(cluster);
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
        // Rotate
        if (_draggedCluster != null)
        {
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
                
                EndDrag?.Invoke(this, new ClusterDraggedArgs(_draggedCluster, mouseState));
                
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
    
    private void MoveClusterToFront(DraggablePartCluster cluster)
    {
        _clusters.Remove(cluster);
        _clusters.AddFirst(cluster);
    }
}