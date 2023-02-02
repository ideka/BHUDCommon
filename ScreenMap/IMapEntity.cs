using Microsoft.Xna.Framework.Graphics;

namespace Ideka.BHUDCommon
{
    public interface IMapEntity
    {
        void DrawToMap(SpriteBatch spriteBatch, IMapBounds map);
    }
}
