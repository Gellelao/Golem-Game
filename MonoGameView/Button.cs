using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView;

public class Button : Region
{
    private readonly Action _action;
    private bool _pressed;

    public Button(Vector2 position, int height, int width, Texture2D texture, Action action) : base(position, height, width, texture)
    {
        _action = action;
    }

    public void Update(MouseState mouseState)
    {
        if(_pressed && mouseState.LeftButton == ButtonState.Released && PointInBounds(mouseState.Position))
        {
            _action.Invoke();
        }
        _pressed = mouseState.LeftButton == ButtonState.Pressed && PointInBounds(mouseState.Position);
    }
}