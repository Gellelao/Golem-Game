using System;
using System.Collections.Generic;
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
        private Shop _shop;
        private IGolemApiClient _client;
        private GolemGrid _grid1;
        private GolemGrid _grid2;
        private List<GolemGrid> _grids;
        private Button _combatButton;
        private SpriteFont _arialFont;
        private PartValidator _validator;
        private ClusterManager _clusterManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _client = GolemApiClientFactory.Create();
            _clusterManager = new ClusterManager();
            _grids = new List<GolemGrid>();
            
            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();
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

            _combatButton = new Button(new Vector2(450, 200), 20, 40, buttonTexture, () => PrintOutcome(golem1, golem2, partsCache));
            
            _shop = new Shop(partsCache);
            var partSelection = _shop.GetPartsForRound();

            _validator = new PartValidator(partsCache);
            
            Constants.SocketDistanceFromLeft = 200;
            _grid1 = new GolemGrid(golem1, _validator, blankTexture, yellowTexture);
            Constants.SocketDistanceFromLeft = 500;
            _grid2 = new GolemGrid(golem2, _validator, blankTexture, yellowTexture);

            _grids.Add(_grid1);
            _grids.Add(_grid2);
            
            foreach (var grid in _grids)
            {
                grid?.SubscribeToClusterEvents(_clusterManager);
            }

            var grayTexture = new Texture2D(GraphicsDevice, 1, 1);
            grayTexture.SetData(new[] { Color.DarkSlateGray });
            
            var redTexture = new Texture2D(GraphicsDevice, 1, 1);
            redTexture.SetData(new[] { Color.IndianRed });
            
            for (var i = 0; i < partSelection.Count; i++)
            {
                _clusterManager.AddCluster(new DraggablePartCluster(new Vector2(50+ i*125, _graphics.PreferredBackBufferHeight-120), grayTexture, _arialFont, redTexture, partSelection[i]));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            var kstate = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                kstate.IsKeyDown(Keys.Escape))
                Exit();

            _clusterManager.Update(mouseState);
            _combatButton.Update(mouseState);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            foreach (var grid in _grids)
            {
                grid?.Draw(_spriteBatch);
            }
            _combatButton.Draw(_spriteBatch);
            _clusterManager.DrawClusters(_spriteBatch);
            
            _spriteBatch.End();

            base.Draw(gameTime);
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