using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView.Buttons;

public class AsyncButton : BaseButton
{
    private readonly Func<Task> _func;

    public AsyncButton(string name, Vector2 position, int height, int width, Texture2D texture, SpriteFont font, Func<Task> func) : base(name, position, height, width, texture, font)
    {
        _func = func;
    }

    public async Task UpdateAsync(MouseState mouseState)
    {
        if(Pressed && mouseState.LeftButton == ButtonState.Released && PointInBounds(mouseState.Position))
        {
            await _func();
        }
        Pressed = mouseState.LeftButton == ButtonState.Pressed && PointInBounds(mouseState.Position);
    }
}