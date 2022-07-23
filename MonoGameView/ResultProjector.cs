using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView;

public class ResultProjector
{
    private List<string> _results;
    private int _numberOfResultsToDisplay;
    private SpriteFont _arialFont;
    private Button _nextButton;
    private Button _endButton;

    public ResultProjector(SpriteFont arialFont, Texture2D buttonTexture)
    {
        _arialFont = arialFont;
        _results = new List<string>();
        _numberOfResultsToDisplay = 1;
        _nextButton = new Button("Next", new Vector2(800, 500), 30, 30, buttonTexture, arialFont, Next);
        _endButton = new Button("End", new Vector2(850, 500), 30, 30, buttonTexture, arialFont, End);
    }

    public void SetResults(List<string> results)
    {
        _numberOfResultsToDisplay = 1;
        _results = results;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!_results.Any() || _numberOfResultsToDisplay > _results.Count) return;
        for (int i = 0; i < _numberOfResultsToDisplay; i++)
        {
            var position = new Vector2(800, 120 + i * 20);
            spriteBatch.DrawString(_arialFont, _results[i], position, Color.Black);
        }
        _nextButton.Draw(spriteBatch);
        _endButton.Draw(spriteBatch);
    }

    public void Update(MouseState mouseState)
    {
        _nextButton.Update(mouseState);
        _endButton.Update(mouseState);
    }

    public void Next()
    {
        if (_numberOfResultsToDisplay < _results.Count)
        {
            _numberOfResultsToDisplay++;
        }
    }

    public void End()
    {
        _numberOfResultsToDisplay = _results.Count;
    }

    public void Clear()
    {
        _numberOfResultsToDisplay = 0;
    }
}