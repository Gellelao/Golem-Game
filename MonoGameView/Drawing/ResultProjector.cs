using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameView.Buttons;

namespace MonoGameView;

public class ResultProjector
{
    private List<string> _results;
    private int _numberOfResultsToDisplay;
    private readonly SpriteFont _arialFont;
    private readonly Button _clearButton;

    public ResultProjector(SpriteFont arialFont, Texture2D buttonTexture)
    {
        _arialFont = arialFont;
        _results = new List<string>();
        _numberOfResultsToDisplay = 1;
        _clearButton = new Button("Clear", new Vector2(800, 500), 30, 30, buttonTexture, arialFont, Clear);
    }

    public void SetResults(List<string> results)
    {
        _results = results;
        _numberOfResultsToDisplay = _results.Count;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!_results.Any() || _numberOfResultsToDisplay > _results.Count) return;
        for (int i = 0; i < _numberOfResultsToDisplay; i++)
        {
            var position = new Vector2(800, 120 + i * 20);
            spriteBatch.DrawString(_arialFont, _results[i], position, Color.Black);
        }
        _clearButton.Draw(spriteBatch);
    }

    public void Update(MouseState mouseState)
    {
        _clearButton.Update(mouseState);
    }

    public void Clear()
    {
        _numberOfResultsToDisplay = 0;
    }
}