using System;
using System.Collections.Generic;
using System.Linq;
using GolemCore.Models.Golem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView;

public class GolemGrid
{
    private Golem _golem;
    private List<PartSocket> _sockets;

    public GolemGrid(Golem golem, Texture2D blankTexture, Texture2D highlightTexture)
    {
        _golem = golem;
        _sockets = new List<PartSocket>();
        
        for(var x = 0; x < _golem.PartIds.Length; x++)
        {
            for(var y = 0; y < _golem.PartIds[x].Length; y++)
            {
                _sockets.Add(new PartSocket(new Vector2(x, y), 70, 70, blankTexture, highlightTexture));
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
            // TODO: Why do X and Y need to be flipped
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

    public bool HighlightCandidateSockets(Point mousePosition)
    {
        var socket = GetSocketUnderMouse(mousePosition);
        if (socket == null || socket.StoredPart != null) return false;
        socket.Highlight = true;
        return true;
    }

    public void ClearHighlights()
    {
        _sockets.ForEach(s => s.Highlight = false);
    }

    private PartSocket GetSocketUnderMouse(Point mousePosition)
    {
        return _sockets.FirstOrDefault(socket => socket.PointInBounds(mousePosition));
    }
}