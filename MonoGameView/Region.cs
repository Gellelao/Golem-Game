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

        public bool PointInBounds(Point mousePosition)
        {
            return mousePosition.X > Position.X &&
                   mousePosition.X < Position.X + Width &&
                   mousePosition.Y > Position.Y &&
                   mousePosition.Y < Position.Y + Height;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, new Rectangle((int)Position.X, (int)Position.Y, Width, Height), Color.White);
        }
    }
}