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
        private Draggable _draggedItem;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _draggables = new List<Draggable>();
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

            var blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            blankTexture.SetData(new[] { Color.DarkSlateGray });
            for (int i = 50; i < 300; i+=25)
            {
                _draggables.Add(new Draggable(new Vector2(i, _graphics.PreferredBackBufferHeight / 2), 20, 20, blankTexture));
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

            if (_draggedItem == null && mouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (var draggable in _draggables)
                {
                    if (draggable.MouseInBounds(mouseState.Position))
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
                    _draggedItem.Release();
                    _draggedItem = null;
                }
            }
            
            _draggables.ForEach(d => d.Update(mouseState));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _draggables.ForEach(d => d.Draw(_spriteBatch));
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}