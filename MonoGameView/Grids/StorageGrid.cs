using GolemCore.Validation;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameView.Grids;

public class StorageGrid : Grid
{
    public StorageGrid(int width, int height, PartValidator validator, Texture2D blankTexture, Texture2D highlightTexture) : base(width, height, validator, blankTexture, highlightTexture)
    {
    }

    protected override void UpdateSource()
    {
        // Storage grids shouldn't do anything when parts are added/removed
    }
}