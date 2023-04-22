using Gw2Sharp.WebApi.V2.Models;
using Ideka.NetCommon;
using Microsoft.Xna.Framework;

namespace Ideka.BHUDCommon;

public static class MapExtensions
{
    public static Vector2 WorldInchesToMap(this Map map, Vector3 coords)
        => new Vector2(
            (float)(map.ContinentRect.TopLeft.X + (coords.X - map.MapRect.TopLeft.X) / map.MapRect.Width * map.ContinentRect.Width),
            (float)(map.ContinentRect.TopLeft.Y - (coords.Y - map.MapRect.TopLeft.Y) / map.MapRect.Height * map.ContinentRect.Height));

    public static Vector2 WorldMetersToMap(this Map map, Vector3 coords)
        => map.WorldInchesToMap(coords * MathUtils.MetersToInches);

    public static Vector3 MapToWorldInches(this Map map, Vector2 coords)
        => new Vector3(
            (float)(map.MapRect.TopLeft.X + (coords.X - map.ContinentRect.TopLeft.X) / map.ContinentRect.Width * map.MapRect.Width),
            (float)(map.MapRect.TopLeft.Y - (coords.Y - map.ContinentRect.TopLeft.Y) / map.ContinentRect.Height * map.MapRect.Height),
            0);

    public static Vector3 MapToWorldMeters(this Map map, Vector2 coords)
        => map.MapToWorldInches(coords) * MathUtils.InchesToMeters;
}
