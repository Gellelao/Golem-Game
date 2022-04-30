using System;
using System.Collections.Generic;
using System.Linq;
using GolemCore.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView;

public class DraggablePartCluster
{
    private readonly DraggablePart[][] _draggableParts;
    private bool _beingDragged;
    private float _xOffsetFromMouse;
    private float _yOffsetFromMouse;
    private Vector2 _position;
    private readonly Texture2D _redTexture;

    public DraggablePartCluster(Vector2 position, Texture2D grayTexture, SpriteFont arialFont, Texture2D redTexture, Part part)
    {
        _position = position;
        _draggableParts = new DraggablePart[part.Shape.Length][];
        _redTexture = redTexture;
        for (var x = 0; x < part.Shape.Length; x++)
        {
            for (var y = 0; y < part.Shape[x].Length; y++)
            {
                _draggableParts[y] ??= new DraggablePart[part.Shape[y].Length];
                if (part.Shape[x][y])
                {
                    _draggableParts[x][y] = new DraggablePart(new Vector2(position.X + x * Constants.PartSize, position.Y + y * Constants.PartSize),
                        Constants.PartSize, grayTexture, arialFont, redTexture)
                    {
                        Part = part
                    };
                }
                else
                {
                    _draggableParts[x][y] = null;
                }
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var line in _draggableParts)
        {
            foreach (var part in line.Where(p => p != null))
            {
                part.Draw(spriteBatch);
            }
        }
        spriteBatch.Draw(_redTexture, _position, new Rectangle((int)_position.X, (int)_position.Y, 10, 10), Color.White);
    }

    public void Update(MouseState mouseState)
    {
        if (_beingDragged)
        {
            _position = new Vector2(mouseState.X - _xOffsetFromMouse, mouseState.Y - _yOffsetFromMouse);
            
            for (var x = 0; x < _draggableParts.Length; x++)
            {
                for (var y = 0; y < _draggableParts[x].Length; y++)
                {
                    if (_draggableParts[x][y] != null)
                    {
                        _draggableParts[x][y].Position = new Vector2(_position.X + x * Constants.PartSize, _position.Y + y * Constants.PartSize);
                    }
                }
            }
        }
    }

    public DraggablePart GetDraggableUnderMouse(Point mousePosition)
    {
        foreach (var line in _draggableParts)
        {
            foreach (var part in line.Where(p => p != null))
            {
                if (part.PointInBounds(mousePosition)) return part;
            }
        }
        
        return null;
    }
    

    public void Grab(MouseState mouseState)
    {
        _beingDragged = true;
        _xOffsetFromMouse = mouseState.X - _position.X;
        _yOffsetFromMouse = mouseState.Y - _position.Y;
    }

    public void Release()
    {
        _beingDragged = false;
        _xOffsetFromMouse = 0;
        _yOffsetFromMouse = 0;
    }

    public bool Contains(DraggablePart partToCheck)
    {
        return _draggableParts.SelectMany(line => line.Where(p => p != null)).Any(part => part == partToCheck);
    }

    public Vector2 GetCoordsForPart(DraggablePart part)
    {
        for (var x = 0; x < _draggableParts.Length; x++)
        {
            for (var y = 0; y < _draggableParts[x].Length; y++)
            {
                if (_draggableParts[x][y] == part)
                {
                    return new Vector2(x, y);
                }
            }
        }

        throw new KeyNotFoundException($"Could not find part {part.Part.Name} in DraggablePartCluster");
    }

    public DraggablePart GetDraggableAtCoords(int x, int y)
    {
        return _draggableParts[x][y];
    }

    public void SetPosition(Vector2 position)
    {
        _position = position;
    }

    public void ClearInvalidDisplay()
    {
        foreach (var line in _draggableParts)
        {
            foreach (var part in line.Where(p => p != null))
            {
                part.Invalid = false;
            }
        }
    }
}