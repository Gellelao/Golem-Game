﻿using System;
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
        private readonly LinkedList<DraggablePartCluster> _clusters;
        private DraggablePart _draggedPart;
        private Shop _shop;
        private IGolemApiClient _client;
        private GolemGrid _grid1;
        private GolemGrid _grid2;
        private Button _combatButton;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _clusters = new LinkedList<DraggablePartCluster>();
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
            var yellowTexture = new Texture2D(GraphicsDevice, 1, 1);
            yellowTexture.SetData(new[] { Color.LightYellow });
            
            var golem1 = new Golem{UserId = 1};
            var golem2 = new Golem{UserId = 2};

            _grid1 = new GolemGrid(golem1, blankTexture, yellowTexture);
            Constants.SocketDistanceFromLeft = 500;
            _grid2 = new GolemGrid(golem2, blankTexture, yellowTexture);
            
            var buttonTexture = new Texture2D(GraphicsDevice, 1, 1);
            buttonTexture.SetData(new[] { Color.ForestGreen });
            
            // Do this now so that its not null by the time Draw() is called
            _combatButton = new Button(new Vector2(350, 200), 20, 40, buttonTexture, null);
            
            var arialFont = Content.Load<SpriteFont>("Arial");

            var parts = await _client.GetParts(new CancellationToken());
            var partsCache = new PartsCache(parts);

            _combatButton = new Button(new Vector2(350, 200), 20, 40, buttonTexture, () => PrintOutcome(golem1, golem2, partsCache));
            
            _shop = new Shop(partsCache);

            var partSelection = _shop.GetPartsForRound(1);

            var grayTexture = new Texture2D(GraphicsDevice, 1, 1);
            grayTexture.SetData(new[] { Color.DarkSlateGray });
            
            var redTexture = new Texture2D(GraphicsDevice, 1, 1);
            redTexture.SetData(new[] { Color.IndianRed });
            
            for (var i = 0; i < partSelection.Count; i++)
            {
                _clusters.AddFirst(new DraggablePartCluster(new Vector2(50+ i*125, _graphics.PreferredBackBufferHeight-120), grayTexture, arialFont, redTexture, partSelection[i]));
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
            _combatButton.Update(mouseState);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _grid1.Draw(_spriteBatch);
            _grid2.Draw(_spriteBatch);
            _combatButton.Draw(_spriteBatch);
            foreach (var cluster in _clusters.Reverse())
            {
                cluster.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void HandleDragging(MouseState mouseState)
        {
            if (_draggedPart != null)
            {
                _grid1.ClearHighlights();
                _grid2.ClearHighlights();
                _draggedPart.Invalid = !_grid1.HighlightCandidateSockets(mouseState.Position) &&
                !_grid2.HighlightCandidateSockets(mouseState.Position);
            }
            
            if (_draggedPart == null && mouseState.LeftButton == ButtonState.Pressed)
            {
                AttemptNewDrag(mouseState);
            }
            
            if (mouseState.LeftButton == ButtonState.Released)
            {
                ReleaseDraggedPart(mouseState);
            }
            
            foreach (var draggable in _clusters)
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
                
                _grid1.ClearHighlights();
                _grid2.ClearHighlights();
                
                _draggedPart.Release();
                _draggedPart.Invalid = false;
                _draggedPart = null;
            }
        }

        private void AttemptNewDrag(MouseState mouseState)
        {
            // foreach (var draggable in _clusters)
            // {
            //     if (draggable.PointInBounds(mouseState.Position))
            //     {
            //         _draggedPart = draggable;
            //         MoveDraggableToFront(draggable);
            //         draggable.Grab(mouseState);
            //         break;
            //     }
            // }
        }

        private void MoveDraggableToFront(DraggablePart draggable)
        {
            // _clusters.Remove(draggable);
            // _clusters.AddFirst(draggable);
        }

        private void PrintOutcome(Golem golem1, Golem golem2, PartsCache cache)
        {
            var results = Resolver.GetOutcome(golem1, golem2, cache);

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }
    }
}