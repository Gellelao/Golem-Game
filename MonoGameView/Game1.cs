using System.Collections.Generic;
using System.Threading;
using GolemCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private List<DraggablePart> _draggables;
        private List<DragSocket> _sockets;
        private DraggablePart _draggedItem;
        private Shop _shop;
        private IGolemApiClient _client;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _draggables = new List<DraggablePart>();
            _sockets = new List<DragSocket>();
        }

        protected override void Initialize()
        {
            _client = GolemApiClientFactory.Create();
            base.Initialize();
        }

        protected override async void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            var arialFont = Content.Load<SpriteFont>("Arial");

            var parts = await _client.GetParts(new CancellationToken());
            _shop = new Shop(new PartsCache(parts));

            var partSelection = _shop.GetPartsForRound(0);

            var grayTexture = new Texture2D(GraphicsDevice, 1, 1);
            grayTexture.SetData(new[] { Color.DarkSlateGray });
            
            for (int i = 0; i < partSelection.Count; i++)
            {
                _draggables.Add(new DraggablePart(new Vector2(i*55, _graphics.PreferredBackBufferHeight / 2), 50, 50, grayTexture, arialFont)
                {
                    Part = partSelection[i]
                });
            }
            
            var blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            blankTexture.SetData(new[] { Color.White });
            for (int i = 50; i < 250; i+=90)
            {
                _sockets.Add(new DragSocket(new Vector2(i, _graphics.PreferredBackBufferHeight / 3), 70, 70, blankTexture));
            }

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