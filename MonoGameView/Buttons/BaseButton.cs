using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView.Buttons;

public abstract class BaseButton : Region
{
    private readonly string _name;
    private readonly SpriteFont _font;
    protected bool Pressed;
    
    public BaseButton(string name, Vector2 position, int height, int width, Texture2D texture, SpriteFont font) : base(position, height, width, texture)
    {
        _name = name;
        _font = font;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        spriteBatch.DrawString(_font, _name, Position, Color.White);
    }

    public abstract void Update(MouseState mouseState);
}