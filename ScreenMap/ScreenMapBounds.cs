using Microsoft.Xna.Framework;

namespace Ideka.BHUDCommon;

public class ScreenMapBounds(MapData mapData) : MapBounds
{
    public override Vector2 FromWorld(int mapId, Vector3 worldMeters)
        => mapData.WorldToScreenMap(mapId, worldMeters);

    public override Vector2 FromMap(Vector2 mapCoords)
        => MapData.MapToScreenMap(mapCoords);
}
