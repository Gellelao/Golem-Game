using GolemCore.Validation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameView.Grids;

public class StorageGrid : Grid
{
    public StorageGrid(int width, int height, Texture2D blankTexture, Texture2D highlightTexture) : base(width, height, blankTexture, highlightTexture)
    {
    }

    protected override void UpdateSource(bool doValidation)
    {
        // Storage grids shouldn't do anything when parts are added/removed
    }
}