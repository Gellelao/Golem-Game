using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GolemCore;
using GolemCore.Models.Golem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private LinkedList<DraggablePart> _draggables;
        private DraggablePart _draggedPart;
        private Shop _shop;
        private IGolemApiClient _client;
        private GolemGrid _grid1;
        private GolemGrid _grid2;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _draggables = new LinkedList<DraggablePart>();
        }

        protected override void Initialize()
        {
            _client = GolemApiClientFactory.Create();
            base.Initialize();
        }

        protected override async void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            

            var blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            blankTexture.SetData(new[] { Color.White });
            
            var golem1 = new Golem{UserId = 1};
            var golem2 = new Golem{UserId = 2};

            _grid1 = new GolemGrid(golem1, blankTexture);
            Constants.SocketDistanceFromLeft = 570;
            _grid2 = new GolemGrid(golem2, blankTexture);
            
            
            var arialFont = Content.Load<SpriteFont>("Arial");

            var parts = await _client.GetParts(new CancellationToken());
            _shop = new Shop(new PartsCache(parts));

            var partSelection = _shop.GetPartsForRound(0);

            var grayTexture = new Texture2D(GraphicsDevice, 1, 1);
            grayTexture.SetData(new[] { Color.DarkSlateGray });
            
            for (var i = 0; i < partSelection.Count; i++)
            {
                _draggables.AddFirst(new DraggablePart(new Vector2(50+ i*65, _graphics.PreferredBackBufferHeight-50), 60, 60, grayTexture, arialFont)
                {
                    Part = partSelection[i]
                });
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
            _grid1.Draw(_spriteBatch);
            _grid2.Draw(_spriteBatch);
            foreach (var draggable in _draggables.Reverse())
            {
                draggable.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void HandleDragging(MouseState mouseState)
        {
            if (_draggedPart == null && mouseState.LeftButton == ButtonState.Pressed)
            {
                AttemptNewDrag(mouseState);
            }
            
            if (mouseState.LeftButton == ButtonState.Released)
            {
                ReleaseDraggedPart(mouseState);
            }
            
            foreach (var draggable in _draggables)
            {
                draggable.Update(mouseState);
            }
        }

        private void ReleaseDraggedPart(MouseState mouseState)
        {
            if (_draggedPart != null)
            {
                _grid1.UnsocketPart(_draggedPart);
                _grid2.UnsocketPart(_draggedPart);

                _grid1.SocketPartAtMouse(mouseState, _draggedPart);
                _grid2.SocketPartAtMouse(mouseState, _draggedPart);
                
                _draggedPart.Release();
                _draggedPart = null;
            }
        }

        private void AttemptNewDrag(MouseState mouseState)
        {
            foreach (var draggable in _draggables)
            {
                if (draggable.PointInBounds(mouseState.Position))
                {
                    _draggedPart = draggable;
                    MoveDraggableToFront(draggable);
                    draggable.Grab(mouseState);
                    break;
                }
            }
        }

        private void MoveDraggableToFront(DraggablePart draggable)
        {
            _draggables.Remove(draggable);
            _draggables.AddFirst(draggable);
        }
    }
}