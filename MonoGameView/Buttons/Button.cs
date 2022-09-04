using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView.Buttons;

public class Button : BaseButton
{
    private readonly Action _action;

    public Button(string name, Vector2 position, int height, int width, Texture2D texture, SpriteFont font, Action action) : base(name, position, height, width, texture, font)
    {
        _action = action;
    }

    public override void Update(MouseState mouseState)
    {
        if(Pressed && mouseState.LeftButton == ButtonState.Released && PointInBounds(mouseState.Position))
        {
            _action.Invoke();
        }
        Pressed = mouseState.LeftButton == ButtonState.Pressed && PointInBounds(mouseState.Position);
    }
}