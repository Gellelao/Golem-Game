using System;
using GolemCore.Extensions;
using GolemCore.Models.Part;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameView;

public class ShopPart : Region
{
    private readonly Part _part;
    private readonly SpriteFont _font;
    private readonly Button _button;

    public ShopPart(Part part, SpriteFont font, Action buttonAction, Vector2 position, int height, int width, Texture2D texture, Texture2D buttonTexture) : base(position, height, width, texture)
    {
        _part = part;
        _font = font;
        _button = new Button(new Vector2(Position.X+100, Position.Y+60), 30, 30, buttonTexture, buttonAction);
    }

    public void Update(MouseState mouseState)
    {
        _button.Update(mouseState);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
        var newPos = new Vector2(Position.X + 2, Position.Y);
        spriteBatch.DrawString(_font, $"{_part.Name}\n{_part.GetDescription()}", newPos, Color.Black);
        _button.Draw(spriteBatch);
    }
}