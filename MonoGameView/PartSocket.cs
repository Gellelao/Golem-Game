using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameView
{
    public class PartSocket : Region
    {
        public Vector2 GolemPartIndex { get; }
        public DraggablePart StoredPart { get; private set; }

        public PartSocket(Vector2 golemPartIndex, int height, int width, Texture2D texture) : base(ConvertIndexToPosition(golemPartIndex), height, width, texture)
        {
            GolemPartIndex = golemPartIndex;
        }

        private static Vector2 ConvertIndexToPosition(Vector2 golemPartIndex)
        {
            return new Vector2(
                Constants.SocketDistanceFromLeft + golemPartIndex.X * Constants.SocketSize,
                Constants.SocketDistanceFromTop + golemPartIndex.Y * Constants.SocketSize
            );
        }

        private Vector2 GetPositionForDraggable(DraggablePart item)
        {
            return new Vector2
            (
                Position.X + Width / 2 - item.Width / 2,
                Position.Y + Height / 2 - item.Height / 2
            );
        }

        public void StorePart(DraggablePart draggedPart)
        {
            StoredPart = draggedPart;
            draggedPart.SetPosition(GetPositionForDraggable(draggedPart));
        }

        public void ClearStorage()
        {
            StoredPart = null;
        }
    }
}