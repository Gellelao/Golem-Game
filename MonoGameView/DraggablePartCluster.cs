using System;
using System.Linq;
using GolemCore.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView;

public class DraggablePartCluster
{
    private readonly DraggablePart[][] _draggableParts;

    public DraggablePartCluster(Vector2 position, Texture2D grayTexture, SpriteFont arialFont, Texture2D redTexture, Part part)
    {
        _draggableParts = new DraggablePart[part.Shape.Length][];
        for (var x = 0; x < part.Shape.Length; x++)
        {
            for (var y = 0; y < part.Shape[x].Length; y++)
            {
                _draggableParts[y] ??= new DraggablePart[part.Shape[y].Length];
                if (part.Shape[x][y])
                {
                    Console.WriteLine($"Creating {part.Name} at {position.X + y * Constants.PartSize},{position.Y + x * Constants.PartSize}");
                    _draggableParts[x][y] = new DraggablePart(new Vector2(position.X + y * Constants.PartSize, position.Y + x * Constants.PartSize),
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
    }

    public void Update(MouseState mouseState)
    {
        foreach (var line in _draggableParts)
        {
            foreach (var part in line.Where(p => p != null))
            {
                part.Update(mouseState);
            }
        }
    }
}