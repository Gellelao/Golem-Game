using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView
{
    public class Draggable
    {
        private Vector2 _position;
        private int _height;
        private int _width;
        private Texture2D _texture;
        private bool _beingDragged = false;
        private float _xOffsetFromMouse;
        private float _yOffsetFromMouse;

        public Draggable(Vector2 position, int height, int width, Texture2D texture)
        {
            _position = position;
            _height = height;
            _width = width;
            _texture = texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, new Rectangle((int)_position.X, (int)_position.Y, _width, _height), Color.White);
        }

        public void Update(MouseState mouseState)
        {
            if (_beingDragged)
            {
                _position = new Vector2(mouseState.X - _xOffsetFromMouse, mouseState.Y - _yOffsetFromMouse);
            }
        }

        public bool MouseInBounds(Point mousePosition)
        {
            return mousePosition.X > _position.X &&
                   mousePosition.X < _position.X + _width &&
                   mousePosition.Y > _position.Y &&
                   mousePosition.Y < _position.Y + _height;
        }

        public void Grab(MouseState mouseState)
        {
            _beingDragged = true;
            _xOffsetFromMouse = mouseState.X - _position.X;
            _yOffsetFromMouse = mouseState.Y - _position.Y;
        }

        public void Release()
        {
            _beingDragged = false;
        }
    }
}