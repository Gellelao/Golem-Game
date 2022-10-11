using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameView.ScreenMessages;

public class Tooltip : Region
{
    private readonly string _text;
    private readonly SpriteFont _font;

    public Tooltip(string text, Vector2 position, Texture2D texture, SpriteFont font) : base(position, 20, 20, texture)
    {
        _text = text;
        _font = font;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        spriteBatch.DrawString(_font, _text, Position, Color.Black);
    }
}