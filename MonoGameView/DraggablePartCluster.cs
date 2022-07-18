using System;
using System.Linq;
using GolemCore.Extensions;
using GolemCore.Models.Part;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView;

public class DraggablePartCluster
{
    public Part Part { get; }
    public bool DraggingEnabled { get; set; }

    private DraggablePart[][] _draggableParts;
    private bool _beingDragged;
    private float _xOffsetFromMouse;
    private float _yOffsetFromMouse;
    private Vector2 _dragOrigin;
    private Vector2 _position;
    private readonly SpriteFont _arialFont;
    private readonly Texture2D _redTexture;

    public DraggablePartCluster(Vector2 position, Texture2D grayTexture, SpriteFont arialFont, Texture2D redTexture,
        Part part)
    {
        _position = position;
        _arialFont = arialFont;
        _draggableParts = new DraggablePart[part.Shape.Length][];
        _redTexture = redTexture;
        DraggingEnabled = true;
        Part = part;
        for (var x = 0; x < part.Shape.Length; x++)
        {
            for (var y = 0; y < part.Shape[x].Length; y++)
            {
                _draggableParts[y] ??= new DraggablePart[part.Shape[y].Length];
                if (part.Shape[x][y])
                {
                    _draggableParts[x][y] = new DraggablePart(new Vector2(position.X + x * Constants.PartSize, position.Y + y * Constants.PartSize),
                        Constants.PartSize, grayTexture, arialFont, redTexture, this)
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
        spriteBatch.DrawString(_arialFont, $"{Part.Name}\n{Part.GetDescription()}", _position, Color.SkyBlue);
    }

    public void Update(MouseState mouseState)
    {
        if (_beingDragged)
        {
            _position = new Vector2(mouseState.X - _xOffsetFromMouse, mouseState.Y - _yOffsetFromMouse);

            UpdatePartPositionsToMatchCluster();
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

    public bool Grab(MouseState mouseState)
    {
        if (!DraggingEnabled) return false;
        _beingDragged = true;
        _dragOrigin = _position;
        _xOffsetFromMouse = mouseState.X - _position.X;
        _yOffsetFromMouse = mouseState.Y - _position.Y;
        return true;
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

    public Vector2 MousePositionToPartCoords(Point mouseCoords)
    {
        var relativeX = mouseCoords.X - _position.X;
        var relativeY = mouseCoords.Y - _position.Y;
        var indexX = (float) Math.Floor(relativeX / Constants.PartSize);
        var indexY = (float) Math.Floor(relativeY / Constants.PartSize);
        return new Vector2(indexX, indexY);
    }

    public DraggablePart GetDraggableAtCoords(int x, int y)
    {
        if (x >= _draggableParts.Length || y >= _draggableParts[0].Length) return null;
        return _draggableParts[x][y];
    }

    public void SetPosition(Vector2 position)
    {
        _position = position;
    }

    public void SetInvalidOnAllParts(bool invalid)
    {
        foreach (var line in _draggableParts)
        {
            foreach (var part in line.Where(p => p != null))
            {
                part.Invalid = invalid;
            }
        }
    }

    public void ClearTempInvalids()
    {
        foreach (var line in _draggableParts)
        {
            foreach (var part in line.Where(p => p != null))
            {
                part.TempInvalid = false;
            }
        }
    }

    public void Rotate()
    {
        var rotated = RotateArray(_draggableParts);
        var minimalArray = TrimUnusedRowsColumns(rotated);
        _draggableParts = minimalArray;
    }

    public void RevertToPositionBeforeDrag()
    {
        _position = _dragOrigin;
        UpdatePartPositionsToMatchCluster();
    }

    private DraggablePart[][] RotateArray(DraggablePart[][] draggableParts)
    {
        var height = draggableParts.Length;
        var width = draggableParts[0].Length; // Assuming square grids here
        var newArray = new DraggablePart[height][];

        for (var y=height-1;y>=0;--y)
        {
            for (var x=0;x<width;++x)
            {
                newArray[x] ??= new DraggablePart[width];
                newArray[x][height-1-y] = draggableParts[y][x];
            }
        }

        return newArray;
    }

    private DraggablePart[][] TrimUnusedRowsColumns(DraggablePart[][] draggableParts)
    {
        // while first row is empty, copy to a new array starting from the second row
        // repeat for columns
        var height = draggableParts.Length;
        var width = draggableParts[0].Length; // Assuming square grids here
        var newArray = new DraggablePart[height][];

        int y;
        int x;

        // Find out where the cluster starts in the original array:
        for (y = 0; y < height; y++)
        {
            if (draggableParts[y].Any(p => p != null)) break;
        }
        for (x = 0; x < width; x++)
        {
            if(draggableParts.Select(row  => row[x]).Any(p => p != null)) break;
        }
        
        // Copy pieces from that index in the original, starting from zero in the new array
        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                newArray[j] ??= new DraggablePart[width];
                if(y+i >= height || x+j >= width) continue;
                var part = draggableParts[y + i][x + j];
                newArray[i][j] = part;
            }
        }
        
        return newArray;
    }

    private void UpdatePartPositionsToMatchCluster()
    {
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