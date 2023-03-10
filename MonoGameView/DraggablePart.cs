using GolemCore.Models.Part;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameView
{
    public class DraggablePart : Region
    {
        private readonly SpriteFont _font;
        private readonly Texture2D _invalidTexture;
        public bool TempInvalid { get; set; }
        public bool Invalid { get; set; }
        public Part Part { get; init; }
        public DraggablePartCluster Parent { get; }

        public DraggablePart(Vector2 position, int size, Texture2D texture, SpriteFont font,
            Texture2D invalidTexture, DraggablePartCluster parent) : base(position, size, size, texture)
        {
            Parent = parent;
            _font = font;
            _invalidTexture = invalidTexture;
        }

        public void SetPosition(Vector2 newPos)
        {
            Position = newPos;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Can add another kind of invalid texture in the future then use a switch here
            var texture = Invalid || TempInvalid ? _invalidTexture : Texture;
            spriteBatch.Draw(texture, Position, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), Color.White);
        }

        public override string ToString()
        {
            return Part.Id.ToString();
        }
    }
}