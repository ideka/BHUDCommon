using Blish_HUD;
using Gw2Sharp.WebApi.V2.Models;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Blish_HUD.GameService;

namespace Ideka.BHUDCommon;

public class MapData : ApiCache<int, Map>
{
    public Map? Current { get; private set; }

    public MapData()
    {
        Gw2Mumble.CurrentMap.MapChanged += CurrentMapChanged;
    }

    public string Describe(int mapId)
        => GetMap(mapId)?.Name ?? $"({mapId})";

    public Map? GetMap(int id)
        => Items.TryGetValue(id, out var map) ? map : null;

    public Vector2 WorldToScreenMap(Vector3 worldMeters)
        => WorldToScreenMap(Gw2Mumble.CurrentMap.Id, worldMeters);

    public Vector2 WorldToScreenMap(int mapId, Vector3 worldMeters)
        => WorldToScreenMap(mapId, worldMeters, ScreenMap.Data.MapCenter, ScreenMap.Data.Scale, ScreenMap.Data.MapRotation, ScreenMap.Data.BoundsCenter);

    public Vector2 WorldToScreenMap(int mapId, Vector3 worldMeters, Vector2 mapCenter, float scale, Matrix rotation, Vector2 boundsCenter)
        => GetMap(mapId) is Map map
            ? MapToScreenMap(map.WorldMetersToMap(worldMeters), mapCenter, scale, rotation, boundsCenter)
            : Vector2.Zero;

    public static Vector2 MapToScreenMap(Vector2 mapCoords)
        => MapToScreenMap(mapCoords, ScreenMap.Data.MapCenter, ScreenMap.Data.Scale, ScreenMap.Data.MapRotation, ScreenMap.Data.BoundsCenter);

    public static Vector2 MapToScreenMap(Vector2 mapCoords, Vector2 mapCenter, float scale, Matrix rotation, Vector2 boundsCenter)
        => Vector2.Transform((mapCoords - mapCenter) * scale, rotation) + boundsCenter;

    protected override async Task<IEnumerable<Map>> ApiGetter(CancellationToken ct)
        => await Gw2WebApi.AnonymousConnection.Client.V2.Maps.AllAsync(ct);

    protected override async Task LoadData(int cachedVersion, string cacheFilePath, CancellationToken ct)
    {
        await base.LoadData(cachedVersion, cacheFilePath, ct);
        UpdateCurrent();
    }

    private void UpdateCurrent()
        => Current = Items.TryGetValue(Gw2Mumble.CurrentMap.Id, out var map) ? map : null;

    private void CurrentMapChanged(object sender, ValueEventArgs<int> e) => UpdateCurrent();

    public override void Dispose()
    {
        Gw2Mumble.CurrentMap.MapChanged -= CurrentMapChanged;
        base.Dispose();
    }
}
