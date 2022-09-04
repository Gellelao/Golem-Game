using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameView.Drawing;

public class TempMessage
{
    private readonly string _message;
    private readonly Color _colour;
    private readonly SpriteFont _font;
    private readonly int _durationMillis;
    private Vector2 _position;

    public bool Expired { get; private set; }
    
    public TempMessage(string message, Color colour, SpriteFont font, Vector2 position)
    {
        _message = message;
        _colour = colour;
        _font = font;
        _position = position;
    }

    public TempMessage(string message, Color colour, SpriteFont font, Vector2 position, int durationMillis) : this(message, colour, font, position)
    {
        _durationMillis = durationMillis;
        StartTimer(_durationMillis);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(_font, _message, _position, _colour);
    }

    public void Update()
    {
        if (_durationMillis > 0)
        {
            _position = new Vector2(_position.X, _position.Y - 1);
        }
    }

    public void ExpireTimer()
    {
        Expired = true;
    }

    private void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        ExpireTimer();
    }

    private void StartTimer(int durationMillis)
    {
        var timer = new Timer(durationMillis);
        timer.Elapsed += TimerElapsed;
        timer.Start();
    }
}