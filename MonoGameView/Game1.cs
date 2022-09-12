using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GolemCore;
using GolemCore.Api;
using GolemCore.Extensions;
using GolemCore.Models.Golem;
using GolemCore.Resolver;
using GolemCore.Validation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameView.Buttons;
using MonoGameView.Grids;
using MonoGameView.ScreenMessages;

namespace MonoGameView
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ShopView _shopView;
        private IGolemApiClient _client;
        private List<Grid> _grids;
        private List<Button> _buttons;
        private List<AsyncButton> _asyncButtons;
        private List<TempMessage> _tempMessages;
        private SpriteFont _arialFont;
        private PartValidator _validator;
        private ClusterManager _clusterManager;
        private ResultProjector _resultProjector;
        private Shop _shop;
        private GolemGrid _playerGrid;
        private GolemGrid _opponentGrid;
        private GolemMaterializer _materializer;

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
            _buttons = new List<Button>();
            _asyncButtons = new List<AsyncButton>();
            _tempMessages = new List<TempMessage>();
            
            _graphics.PreferredBackBufferWidth = 1500;
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

            _arialFont = Content.Load<SpriteFont>("Arial");

            var grayTexture = new Texture2D(GraphicsDevice, 1, 1);
            grayTexture.SetData(new[] { Color.DarkSlateGray });
            
            var redTexture = new Texture2D(GraphicsDevice, 1, 1);
            redTexture.SetData(new[] { Color.IndianRed });
            
            _resultProjector = new ResultProjector(_arialFont, grayTexture);

            var parts = await _client.GetParts(new CancellationToken());
            var partsCache = new PartsCache(parts);

            _buttons.Add(new Button("Fight", new Vector2(450, 200), 20, 40, buttonTexture, _arialFont, () => OnFightClicked(golem1, golem2, partsCache)));
            _buttons.Add(new Button("Reroll", new Vector2(450, 400), 20, 40, buttonTexture, _arialFont, () => _shopView.Reroll()));
            var uploadButtonLocation = new Vector2(250, 30);
            _asyncButtons.Add(new AsyncButton("Upload", uploadButtonLocation, 20, 40, buttonTexture, _arialFont, async () => await OnUploadClicked(golem1, uploadButtonLocation)));
            var summonButtonLocation = new Vector2(600, 30);
            _asyncButtons.Add(new AsyncButton("Summon", summonButtonLocation, 20, 40, buttonTexture, _arialFont, async () => await OnSummonClicked(summonButtonLocation)));

            _shop = new Shop(partsCache);

            _clusterManager = new ClusterManager(grayTexture, redTexture, _arialFont);

            _materializer = new GolemMaterializer(partsCache, _clusterManager);

            _shopView = new ShopView(_shop, _clusterManager);

            _validator = new PartValidator(partsCache);
            
            Constants.SocketDistanceFromLeft = 200;
            _playerGrid = new GolemGrid(golem1, _validator, blankTexture, yellowTexture);
            Constants.SocketDistanceFromLeft = 500;
            _opponentGrid = new GolemGrid(golem2, _validator, blankTexture, yellowTexture);
            Constants.SocketDistanceFromLeft = 30;
            var storageGrid = new StorageGrid(2, 6, blankTexture, yellowTexture);
            Constants.SocketDistanceFromLeft = 800;
            var sellGrid = new SellGrid(_shopView, blankTexture, yellowTexture);
            _grids.Add(storageGrid);
            _grids.Add(sellGrid);
            
            foreach (var grid in _grids.Append(_playerGrid).Append(_opponentGrid))
            {
                grid?.SubscribeToClusterEvents(_clusterManager);
            }
        }

        protected override async void Update(GameTime gameTime)
        {
            var kstate = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                kstate.IsKeyDown(Keys.Escape))
                Exit();

            foreach (var button in _buttons)
            {
                button.Update(mouseState);
            }
            foreach (var button in _asyncButtons)
            {
                await button.UpdateAsync(mouseState);
            }
            _clusterManager?.Update(mouseState);
            _resultProjector?.Update(mouseState);
            
            foreach (var grid in _grids.Append(_playerGrid).Append(_opponentGrid))
            {
                grid?.Update(mouseState);
            }

            var messagesToRemove = new List<TempMessage>();
            foreach (var message in _tempMessages)
            {
                message.Update();
                if (message.Expired)
                {
                    messagesToRemove.Add(message);
                }
            }

            _tempMessages.RemoveAll(m => messagesToRemove.Contains(m));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            
            foreach (var grid in _grids.Append(_playerGrid).Append(_opponentGrid))
            {
                grid?.Draw(_spriteBatch);
            }
            foreach (var button in _buttons)
            {
                button.Draw(_spriteBatch);
            }
            foreach (var button in _asyncButtons)
            {
                button.Draw(_spriteBatch);
            }
            foreach (var message in _tempMessages)
            {
                message.Draw(_spriteBatch);
            }
            _clusterManager?.DrawClusters(_spriteBatch);
            _resultProjector?.Draw(_spriteBatch);

            if (_shopView != null)
            {
                _spriteBatch.DrawString(_arialFont, $"{_shopView.GetPlayerFunds()}", new Vector2(20, 20), Color.Black);
            }
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void OnFightClicked(Golem golem1, Golem golem2, PartsCache cache)
        {
            if (!_playerGrid.Valid || !_opponentGrid.Valid) return;
            var resolver = new CombatResolver(golem1, golem2, cache);
            var results = resolver.GetOutcome();

            _resultProjector.SetResults(results);
            
            _shop.IncrementRound();
            _shop.IncrementFunds();
            _shopView.Reroll();
        }

        private async Task OnUploadClicked(Golem golem, Vector2 uploadButtonLocation)
        {
            var messageLocation = new Vector2(uploadButtonLocation.X + 20, uploadButtonLocation.Y);
            if(!_playerGrid.Valid)
            {
                _tempMessages.Add(new TempMessage("Invalid Grid", Color.Red, _arialFont, messageLocation, 1500));
                return;
            }
            var golemRequest = new CreateGolemRequest
            {
                Item = golem
            };

            var loadingMessage = new TempMessage("Uploading...", Color.Yellow, _arialFont, messageLocation);
            _tempMessages.Add(loadingMessage);
            await _client.CreateGolem(golemRequest, new CancellationToken());
            loadingMessage.ExpireTimer();
            _tempMessages.Add(new TempMessage("Success!", Color.Green, _arialFont, messageLocation, 1500));
        }

        private async Task OnSummonClicked(Vector2 summonButtonLocation)
        {
            var messageLocation = new Vector2(summonButtonLocation.X + 20, summonButtonLocation.Y);
            var loadingMessage = new TempMessage("Summoning...", Color.Yellow, _arialFont, messageLocation);
            _tempMessages.Add(loadingMessage);
            var golems = await _client.GetAllGolems(new CancellationToken());
            
            _materializer.Materialize(golems.First(), _opponentGrid);
            
            loadingMessage.ExpireTimer();
            _tempMessages.Add(new TempMessage("Success!", Color.Green, _arialFont, messageLocation, 1500));
        }
    }
}