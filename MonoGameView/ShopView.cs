using System;
using System.Collections.Generic;
using GolemCore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameView.Events;

namespace MonoGameView;

public class ShopView
{
    public event EventHandler<PartTransactionArgs> PartBought;
    public event EventHandler<PartTransactionArgs> PartSold;
    
    private readonly Shop _shop;
    private readonly Texture2D _itemBackround;
    private readonly Texture2D _buttonTexture;
    private readonly SpriteFont _arialFont;
    private List<ShopPart> _shopParts;

    public ShopView(Shop shop, Texture2D yellowTexture, Texture2D buttonTexture,
        SpriteFont arialFont)
    {
        _shop = shop;
        _itemBackround = yellowTexture;
        _buttonTexture = buttonTexture;
        _arialFont = arialFont;
        _shopParts = new List<ShopPart>();
        shop.SetPartsForRound();
        GenerateShopParts();
    }

    public void Update(MouseState mouseState)
    {
        foreach (var shopPart in _shopParts)
        {
            shopPart.Update(mouseState);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var shopPart in _shopParts)
        {
            shopPart.Draw(spriteBatch);
        }
    }

    private void GenerateShopParts()
    {
        var parts = _shop.GetCurrentSelection();
        _shopParts = new List<ShopPart>();
        for (int i = 0; i < parts.Count; i++)
        {
            var indexForThisPart = i; // because closures
            _shopParts.Add(new ShopPart(parts[i], _arialFont, () => BuyPart(indexForThisPart), new Vector2(80 + 80*i, 500), 70, 70, _itemBackround, _buttonTexture));
        }
    }

    private void BuyPart(int index)
    {
        var partBought = _shop.BuyPartAtIndex(index);
        if (partBought == null) return;

        PartBought?.Invoke(this, new PartTransactionArgs(partBought));
        GenerateShopParts();
    }
}