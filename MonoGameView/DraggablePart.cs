using GolemCore.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameView
{
    public class DraggablePart : Region
    {
        private readonly SpriteFont _font;
        private readonly Texture2D _invalidTexture;
        public bool Invalid { get; set; }

        public Part Part { get; init; }

        public DraggablePart(Vector2 position, int size, Texture2D texture, SpriteFont font,
            Texture2D invalidTexture) : base(position, size, size, texture)
        {
            _font = font;
            _invalidTexture = invalidTexture;
        }

        public void SetPosition(Vector2 newPos)
        {
            Position = newPos;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var texture = Invalid ? _invalidTexture : Texture;
            spriteBatch.Draw(texture, Position, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), Color.White);
            spriteBatch.DrawString(_font, Part.Name.Replace(' ', '\n'), Position, Color.White);
        }
    }
}