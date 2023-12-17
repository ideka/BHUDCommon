using Blish_HUD;
using Blish_HUD.Modules.Managers;
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

public abstract class ApiCache<TId, TItem> : IDisposable
    where TItem : IIdentifiable<TId>
{
    private static readonly Logger Logger = Logger.GetLogger<ApiCache<TId, TItem>>();

    private const int Retries = 3;

    private Dictionary<TId, TItem> _items = new();
    public IReadOnlyDictionary<TId, TItem> Items => _items;

    private readonly CancellationTokenSource _cts = new();
    private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
    {
        Converters =
        {
            new RectangleConverter(),
            new Coordinates2Converter(),
        },
    };

    private class CacheFile
    {
        public int BuildId { get; set; }
        public Dictionary<TId, TItem> Items { get; set; } = new();
    }

    public async Task<Task> StartLoad(string cacheFilePath, ContentsManager? contentsManager, string? builtinFilePath)
    {
        CacheFile? cache = null;

        if (File.Exists(cacheFilePath))
        {
            Logger.Info("Found cache file, loading.");
            try
            {
                cache = JsonConvert.DeserializeObject<CacheFile>(
                    File.ReadAllText(cacheFilePath),
                    _jsonSerializerSettings);

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

        _items = cache?.Items ?? new();
        return LoadData(cache?.BuildId ?? 0, cacheFilePath, _cts.Token);
    }

    protected abstract Task<IEnumerable<TItem>> ApiGetter(CancellationToken ct);

    protected virtual async Task LoadData(int cachedVersion, string cacheFilePath, CancellationToken ct)
    {
        for (int i = 0; i < 30; i++)
        {
            if (Gw2Mumble.Info.BuildId != 0)
                break;

            Logger.Warn("Waiting for mumble to update...");
            await Task.Delay(1000);
        }

        if (Gw2Mumble.Info.BuildId == cachedVersion)
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
            return;  // We failed to load data.
        }

        _items = items.ToDictionary(i => i.Id);

        Directory.CreateDirectory(Path.GetDirectoryName(cacheFilePath));
        File.WriteAllText(cacheFilePath, JsonConvert.SerializeObject(new CacheFile()
        {
            BuildId = Gw2Mumble.Info.BuildId != 0 ? Gw2Mumble.Info.BuildId : cachedVersion,
            Items = _items,
        }, _jsonSerializerSettings));
    }

    public virtual void Dispose()
    {
        _cts?.Cancel();
    }
}
