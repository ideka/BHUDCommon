using Blish_HUD;
using Gw2Sharp.WebApi.V2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static Blish_HUD.GameService;
using File = System.IO.File;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Ideka.BHUDCommon;

public abstract class ApiCache<TId, TItem> : IDisposable
    where TItem : IIdentifiable<TId>
{
    protected abstract Logger Logger { get; }

    private const int Retries = 3;

    private readonly Dictionary<TId, TItem> _data = new();
    protected IReadOnlyDictionary<TId, TItem> Data => _data;

    private readonly CancellationTokenSource _cts = new();

    protected readonly object _lock = new object();

    private class CacheFile
    {
        public int BuildId { get; set; }
        public Dictionary<TId, TItem> Data { get; set; } = new();
    }

    public ApiCache(string cacheFilePath)
    {
        CacheFile? cache = null;
        if (File.Exists(cacheFilePath))
        {
            Logger.Info("Found cache file, loading.");
            try
            {
                cache = JsonSerializer.Deserialize<CacheFile>(File.ReadAllText(cacheFilePath));
                if (cache == null)
                    Logger.Warn("Cache load resulted in null.");
            }
            catch (Exception e)
            {
                Logger.Warn(e, "Exception when loading cache.");
            }
        }

        _data = cache?.Data ?? new Dictionary<TId, TItem>();

        _ = LoadData(cache?.BuildId ?? 0, cacheFilePath, _cts.Token);
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

        IEnumerable<TItem>? list = null;

        for (int i = 0; i < Retries; i++)
        {
            try
            {
                var list2 = await ApiGetter(ct);
                list = list2;
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

        if (list == null)
        {
            Logger.Warn($"Max retries exeeded. Skipping {typeof(TItem).FullName} data update.");
            return;  // We failed to load data.
        }

        lock (_lock)
        {
            foreach (var item in list)
                _data[item.Id] = item;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(cacheFilePath));
        File.WriteAllText(cacheFilePath, JsonConvert.SerializeObject(new CacheFile()
        {
            BuildId = Gw2Mumble.Info.BuildId != 0 ? Gw2Mumble.Info.BuildId : cachedVersion,
            Data = _data,
        }));
    }

    public virtual void Dispose()
    {
        _cts?.Cancel();
    }
}
