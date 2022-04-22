using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameView
{
    public class DragSocket : Region
    {
        public DragSocket(Vector2 position, int height, int width, Texture2D texture) : base(position, height, width, texture)
        {
        }

        public Vector2 GetPositionForDraggable(DraggablePart item)
        {
            return new Vector2
            (
                Position.X + Width / 2 - item.Width / 2,
                Position.Y + Height / 2 - item.Height / 2
            );
        }
    }
}