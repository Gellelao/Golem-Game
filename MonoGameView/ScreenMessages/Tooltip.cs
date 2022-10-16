using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameView.ScreenMessages;

public class Tooltip : Region
{
    private readonly string _text;
    private readonly SpriteFont _font;

    public Tooltip(string text, Vector2 position, Texture2D texture, SpriteFont font) : base(position, (int)font.MeasureString(text).Y, (int)font.MeasureString(text).X + 15, texture)
    {
        _text = text;
        _font = font;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        spriteBatch.DrawString(_font, _text, new Vector2(Position.X + 12, Position.Y + 5), Color.Black);
    }
}