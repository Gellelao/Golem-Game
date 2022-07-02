using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GolemCore;
using GolemCore.Models.Golem;
using GolemCore.Validation;
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
        private DraggablePartCluster _draggedCluster;
        private Shop _shop;
        private IGolemApiClient _client;
        private GolemGrid _grid1;
        private GolemGrid _grid2;
        private Button _combatButton;
        private SpriteFont _arialFont;
        private bool _rightMousePressed;
        private PartValidator _validator;

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
            yellowTexture.SetData(new[] { Color.Yellow });
            
            var golem1 = new Golem{UserId = 1};
            var golem2 = new Golem{UserId = 2};
            
            var buttonTexture = new Texture2D(GraphicsDevice, 1, 1);
            buttonTexture.SetData(new[] { Color.ForestGreen });
            
            // Do this now so that its not null by the time Draw() is called
            _combatButton = new Button(new Vector2(350, 200), 20, 40, buttonTexture, null);
            
            _arialFont = Content.Load<SpriteFont>("Arial");

            var parts = await _client.GetParts(new CancellationToken());
            var partsCache = new PartsCache(parts);

            _combatButton = new Button(new Vector2(350, 200), 20, 40, buttonTexture, () => PrintOutcome(golem1, golem2, partsCache));
            
            _shop = new Shop(partsCache);
            var partSelection = _shop.GetPartsForRound(2);

            _validator = new PartValidator(partsCache);
            
            _grid1 = new GolemGrid(golem1, _validator, blankTexture, yellowTexture);
            Constants.SocketDistanceFromLeft = 500;
            _grid2 = new GolemGrid(golem2, _validator, blankTexture, yellowTexture);


            var grayTexture = new Texture2D(GraphicsDevice, 1, 1);
            grayTexture.SetData(new[] { Color.DarkSlateGray });
            
            var redTexture = new Texture2D(GraphicsDevice, 1, 1);
            redTexture.SetData(new[] { Color.IndianRed });
            
            for (var i = 0; i < partSelection.Count; i++)
            {
                _clusters.AddFirst(new DraggablePartCluster(new Vector2(50+ i*125, _graphics.PreferredBackBufferHeight-120), grayTexture, _arialFont, redTexture, partSelection[i]));
                _clusters.AddFirst(new DraggablePartCluster(new Vector2(50+ i*125, _graphics.PreferredBackBufferHeight-120), grayTexture, _arialFont, redTexture, partSelection[i]));
                _clusters.AddFirst(new DraggablePartCluster(new Vector2(50+ i*125, _graphics.PreferredBackBufferHeight-120), grayTexture, _arialFont, redTexture, partSelection[i]));
                _clusters.AddFirst(new DraggablePartCluster(new Vector2(50+ i*125, _graphics.PreferredBackBufferHeight-120), grayTexture, _arialFont, redTexture, partSelection[i]));
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
            _grid1?.Draw(_spriteBatch);
            _grid2?.Draw(_spriteBatch);
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
            if (_draggedCluster != null)
            {
                _grid1.ClearHighlights();
                _grid2.ClearHighlights();
                _grid1.DisplayValidation(mouseState.Position, _draggedCluster);
                _grid2.DisplayValidation(mouseState.Position, _draggedCluster);
                
                if (_rightMousePressed && mouseState.RightButton == ButtonState.Released)
                {
                    _draggedCluster.Rotate();
                }

                _rightMousePressed = mouseState.RightButton == ButtonState.Pressed;
            }
            
            if (_draggedCluster == null && mouseState.LeftButton == ButtonState.Pressed)
            {
                AttemptNewDrag(mouseState);
            }
            
            if (mouseState.LeftButton == ButtonState.Released)
            {
                ReleaseDraggedPart(mouseState);
            }
            
            foreach (var cluster in _clusters)
            {
                cluster.Update(mouseState);
            }
        }

        private void ReleaseDraggedPart(MouseState mouseState)
        {
            if (_draggedCluster != null)
            {
                _draggedCluster.Release();
                
                _grid1.SocketClusterAtMouse(mouseState, _draggedCluster);
                _grid2.SocketClusterAtMouse(mouseState, _draggedCluster);

                _grid1.ClearHighlights();
                _grid2.ClearHighlights();
                
                _draggedCluster.ClearTempInvalids();
                _draggedCluster = null;
            }
        }

        private void AttemptNewDrag(MouseState mouseState)
        {
            foreach (var cluster in _clusters)
            {
                if (cluster.GetDraggableUnderMouse(mouseState.Position) == null) continue;
                
                _draggedCluster = cluster;
                
                _grid1.UnsocketPartsOfCluster(_draggedCluster);
                _grid2.UnsocketPartsOfCluster(_draggedCluster);
                
                MoveClusterToFront(cluster);
                cluster.Grab(mouseState);
                break;
            }
        }

        private void MoveClusterToFront(DraggablePartCluster cluster)
        {
            _clusters.Remove(cluster);
            _clusters.AddFirst(cluster);
        }

        private void PrintOutcome(Golem golem1, Golem golem2, PartsCache cache)
        {
            var results = CombatResolver.GetOutcome(golem1, golem2, cache);

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }
    }
}