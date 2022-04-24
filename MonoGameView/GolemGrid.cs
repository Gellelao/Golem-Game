using System;
using System.Collections.Generic;
using GolemCore.Models.Golem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView;

public class GolemGrid
{
    private Golem _golem;
    private List<PartSocket> _sockets;

    public GolemGrid(Golem golem, Texture2D socketTexture)
    {
        _golem = golem;
        _sockets = new List<PartSocket>();
        
        for(var x = 0; x < _golem.PartIds.Length; x++)
        {
            for(var y = 0; y < _golem.PartIds[x].Length; y++)
            {
                _sockets.Add(new PartSocket(new Vector2(x, y), 70, 70, socketTexture));
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _sockets.ForEach(d => d.Draw(spriteBatch));
    }

    private void UpdateGolem()
    {
        foreach (var socket in _sockets)
        {
            _golem.PartIds[(int)socket.GolemPartIndex.Y][(int)socket.GolemPartIndex.X] = socket.StoredPart == null ? "-1" : socket.StoredPart.Part.Id.ToString();
        }
        Console.WriteLine(_golem);
    }

    public void UnsocketPart(DraggablePart part)
    {
        foreach (var socket in _sockets)
        {
            if (socket.StoredPart == part)
            {
                socket.ClearStorage();
            }
        }
    }

    public void SocketPartAtMouse(MouseState mouseState, DraggablePart part)
    {
        foreach (var socket in _sockets)
        {
            if (socket.StoredPart != null) continue;
            if (!socket.PointInBounds(mouseState.Position)) continue;
                    
            socket.StorePart(part);
            UpdateGolem();
            break;
        }
    }
}