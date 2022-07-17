using System;
using System.Collections.Generic;
using System.Threading;
using GolemCore;
using GolemCore.Models.Golem;
using GolemCore.Validation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameView.Grids;

namespace MonoGameView
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ShopView _shopView;
        private IGolemApiClient _client;
        private List<Grid> _grids;
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
            _grids = new List<Grid>();
            
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

            var grayTexture = new Texture2D(GraphicsDevice, 1, 1);
            grayTexture.SetData(new[] { Color.DarkSlateGray });
            
            var redTexture = new Texture2D(GraphicsDevice, 1, 1);
            redTexture.SetData(new[] { Color.IndianRed });

            var parts = await _client.GetParts(new CancellationToken());
            var partsCache = new PartsCache(parts);

            _combatButton = new Button(new Vector2(450, 200), 20, 40, buttonTexture, () => PrintOutcome(golem1, golem2, partsCache));
            
            var shop = new Shop(partsCache);

            _validator = new PartValidator(partsCache);
            
            Constants.SocketDistanceFromLeft = 200;
            var grid1 = new GolemGrid(golem1, _validator, blankTexture, yellowTexture);
            Constants.SocketDistanceFromLeft = 500;
            var grid2 = new GolemGrid(golem2, _validator, blankTexture, yellowTexture);
            Constants.SocketDistanceFromLeft = 30;
            var storageGrid = new StorageGrid(2, 6, blankTexture, yellowTexture);
            Constants.SocketDistanceFromLeft = 800;
            var sellGrid = new SellGrid(_shopView, blankTexture, yellowTexture);
            _grids.Add(grid1);
            _grids.Add(grid2);
            _grids.Add(storageGrid);
            _grids.Add(sellGrid);

            _clusterManager = new ClusterManager(grayTexture, redTexture, _arialFont);
            
            foreach (var grid in _grids)
            {
                grid?.SubscribeToClusterEvents(_clusterManager);
            }

            _shopView = new ShopView(shop, _clusterManager);
        }

        protected override void Update(GameTime gameTime)
        {
            var kstate = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                kstate.IsKeyDown(Keys.Escape))
                Exit();

            _clusterManager?.Update(mouseState);
            _combatButton.Update(mouseState);

            foreach (var grid in _grids)
            {
                grid?.Update(mouseState);
            }

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
            _clusterManager?.DrawClusters(_spriteBatch);
            
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