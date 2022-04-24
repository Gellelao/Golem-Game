using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameView
{
    public abstract class Region
    {
        public Vector2 Position { get; protected set; }
        public int Height { get; }
        public int Width { get; }
        public Texture2D Texture { get; }

        public Region(Vector2 position, int height, int width, Texture2D texture)
        {
            Position = position;
            Height = height;
            Width = width;
            Texture = texture;
        }

        public bool PointInBounds(Point point)
        {
            return point.X > Position.X &&
                   point.X < Position.X + Width &&
                   point.Y > Position.Y &&
                   point.Y < Position.Y + Height;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), Color.White);
        }
    }
}