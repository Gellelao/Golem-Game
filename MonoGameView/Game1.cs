using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameView;

namespace MonoGameCross_PlatformDesktopApplication1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _ballTexture;
        private Vector2 _ballPosition;
        private float _ballSpeed;
        private List<Draggable> _draggables;
        private List<DragSocket> _sockets;
        private Draggable _draggedItem;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _draggables = new List<Draggable>();
            _sockets = new List<DragSocket>();
        }

        protected override void Initialize()
        {
            _ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
                _graphics.PreferredBackBufferHeight / 2);
            _ballSpeed = 1000f;
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var grayTexture = new Texture2D(GraphicsDevice, 1, 1);
            grayTexture.SetData(new[] { Color.DarkSlateGray });
            
            for (int i = 50; i < 300; i+=40)
            {
                _draggables.Add(new Draggable(new Vector2(i, _graphics.PreferredBackBufferHeight / 2), 50, 50, grayTexture));
            }
            
            var blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            blankTexture.SetData(new[] { Color.White });
            for (int i = 50; i < 250; i+=90)
            {
                _sockets.Add(new DragSocket(new Vector2(i, _graphics.PreferredBackBufferHeight / 3), 70, 70, blankTexture));
            }

            _ballTexture = Content.Load<Texture2D>("ball");
        }

        protected override void Update(GameTime gameTime)
        {
            var kstate = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                kstate.IsKeyDown(Keys.Escape))
                Exit();

            HandleDragging(mouseState);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _sockets.ForEach(d => d.Draw(_spriteBatch));
            _draggables.ForEach(d => d.Draw(_spriteBatch));
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void HandleDragging(MouseState mouseState)
        {

            if (_draggedItem == null && mouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (var draggable in _draggables)
                {
                    if (draggable.PointInBounds(mouseState.Position))
                    {
                        _draggedItem = draggable;
                        draggable.Grab(mouseState);
                        break;
                    }
                }
            }
            
            if (mouseState.LeftButton == ButtonState.Released)
            {
                if (_draggedItem != null)
                {
                    foreach (var socket in _sockets)
                    {
                        if (socket.PointInBounds(mouseState.Position))
                        {
                            _draggedItem.SetPosition(socket.GetPositionForDraggable(_draggedItem));
                            break;
                        }
                    }
                    _draggedItem.Release();
                    _draggedItem = null;
                }
            }
            
            _draggables.ForEach(d => d.Update(mouseState));
        }
    }
}