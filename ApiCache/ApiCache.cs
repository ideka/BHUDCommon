using Blish_HUD;
using Blish_HUD.Modules.Managers;
using Gw2Sharp.WebApi;
using Gw2Sharp.WebApi.V2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Blish_HUD.GameService;
using File = System.IO.File;

namespace Ideka.BHUDCommon;

public static class ApiCache
{
    public static int? TryExtractAssetId(RenderUrl? url)
        => TryExtractAssetId(url?.Url?.ToString());

    public static int? TryExtractAssetId(string? url)
        => url != null && int.TryParse(url.Split('/').Last().Split('.').First(), out int id) ? id : null;
}

public abstract class ApiCache<TId, TItem> : IDisposable
    where TItem : IIdentifiable<TId>
{
    private static readonly Logger Logger = Logger.GetLogger<ApiCache<TId, TItem>>();

    private const int Retries = 3;

    private Dictionary<TId, TItem> _items = [];
    public IReadOnlyDictionary<TId, TItem> Items => _items;

    private readonly CancellationTokenSource _cts = new();
    private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        Converters =
        {
            new RectangleConverter(),
            new Coordinates2Converter(),
            new ApiEnumConverter(),
            new RenderUrlConverter(),
            new NullableRenderUrlConverter(),
        },
    };

    private class CacheFile
    {
        public int BuildId { get; set; }
        public Locale Locale { get; set; }
        public Dictionary<TId, TItem> Items { get; set; } = [];
    }

    public async Task<Task> StartLoad(string cacheFilePath, ContentsManager? contentsManager, string? builtinFilePath,
        CancellationToken ct = default)
    {
        ct.Register(_cts.Cancel);

        CacheFile? cache = null;

        if (File.Exists(cacheFilePath))
        {
            Logger.Info("Found cache file, loading.");
            try
            {
                using var file = File.OpenRead(cacheFilePath);
                using var reader = new StreamReader(file);
                cache = JsonConvert.DeserializeObject<CacheFile>(
                    await reader.ReadToEndAsync(),
                    _jsonSerializerSettings);
                _cts.Token.ThrowIfCancellationRequested();

                if (cache == null || !cache.Items.Any())
                {
                    Logger.Warn("Cache load resulted empty or null.");
                    cache = null;
                }
            }
            catch (Exception e)
            {
                Logger.Warn(e, "Exception when loading cache.");
            }
        }

        if ((cache?.Items.Any() != true) && contentsManager != null && builtinFilePath != null)
        {
            Logger.Info("Loading builtin cache file.");
            try
            {
                using var file = contentsManager.GetFileStream(builtinFilePath);
                using var reader = new StreamReader(file);
                cache = JsonConvert.DeserializeObject<CacheFile>(
                    await reader.ReadToEndAsync(),
                    _jsonSerializerSettings);
                _cts.Token.ThrowIfCancellationRequested();

                if (cache == null || !cache.Items.Any())
                {
                    Logger.Warn("Builtin cache load resulted empty or null.");
                    cache = null;
                }
            }
            catch (Exception e)
            {
                Logger.Warn(e, "Exception when loading builtin cache.");
            }
        }

        _items = cache?.Items ?? [];
        return LoadData(cache?.BuildId, cache?.Locale, cacheFilePath, _cts.Token);
    }

    protected abstract Task<IEnumerable<TItem>> ApiGetter(CancellationToken ct);

    protected virtual async Task LoadData(int? cachedVersion, Locale? cachedLocale, string cacheFilePath,
        CancellationToken ct)
    {
        for (int i = 0; i < 30; i++)
        {
            if (Gw2Mumble.Info.BuildId != 0)
                break;

            await Task.Delay(1000);
        }

        if (Gw2Mumble.Info.BuildId == cachedVersion && Overlay.UserLocale.Value == cachedLocale)
            return;

        IEnumerable<TItem>? items = null;

        for (int i = 0; i < Retries; i++)
        {
            try
            {
                items = await ApiGetter(ct);
                break;
            }
            catch (Exception e)
            {
                if (i < Retries)
                {
                    Logger.Warn(e, "Failed to pull data from the Gw2 API. Trying again in 30 seconds.");
                    await Task.Delay(30000);
                }
            }
        }

        if (items == null)
        {
            Logger.Warn($"Max retries exeeded. Skipping {typeof(TItem).FullName} data update.");
            return;  // We failed to load data
        }

        _items = items.ToDictionary(i => i.Id);

        Directory.CreateDirectory(Path.GetDirectoryName(cacheFilePath));
        using var file = File.OpenWrite(cacheFilePath);
        using var writer = new StreamWriter(file);
        await writer.WriteAsync(JsonConvert.SerializeObject(new CacheFile()
        {
            BuildId = Gw2Mumble.Info.BuildId != 0 ? Gw2Mumble.Info.BuildId : (cachedVersion ?? 0),
            Locale = Overlay.UserLocale.Value,
            Items = _items,
        }, _jsonSerializerSettings));
    }

    public virtual void Dispose()
    {
        _cts?.Cancel();
    }
}
