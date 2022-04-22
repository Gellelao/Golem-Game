using GolemCore.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView
{
    public class DraggablePart : Region
    {
        private readonly SpriteFont _font;
        private bool _beingDragged;
        private float _xOffsetFromMouse;
        private float _yOffsetFromMouse;
        
        public Part Part { get; init; }

        public DraggablePart(Vector2 position, int height, int width, Texture2D texture, SpriteFont font) : base(position, height, width, texture)
        {
            _font = font;
        }

        public void Update(MouseState mouseState)
        {
            if (_beingDragged)
            {
                Position = new Vector2(mouseState.X - _xOffsetFromMouse, mouseState.Y - _yOffsetFromMouse);
            }
        }

        public void Grab(MouseState mouseState)
        {
            _beingDragged = true;
            _xOffsetFromMouse = mouseState.X - Position.X;
            _yOffsetFromMouse = mouseState.Y - Position.Y;
        }

        public void SetPosition(Vector2 newPos)
        {
            Position = newPos;
        }

        public void Release()
        {
            _beingDragged = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.DrawString(_font, Part.Name.Replace(' ', '\n'), Position, Color.White);
        }
    }
}