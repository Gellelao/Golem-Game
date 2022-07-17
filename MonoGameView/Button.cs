using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView;

public class Button : Region
{
    private readonly string _name;
    private readonly SpriteFont _font;
    private readonly Action _action;
    private bool _pressed;

    public Button(string name, Vector2 position, int height, int width, Texture2D texture, SpriteFont font, Action action) : base(position, height, width, texture)
    {
        _name = name;
        _font = font;
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

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        spriteBatch.DrawString(_font, _name, Position, Color.White);
    }
}