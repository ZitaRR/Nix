using Microsoft.Extensions.Caching.Memory;
using System;

namespace Nix.Infrastructure;

public abstract class CacheBase<TKey, TValue> : ICache<TKey, TValue>
{
    protected IMemoryCache Cache { get; } = new MemoryCache(new MemoryCacheOptions { ExpirationScanFrequency = TimeSpan.FromMinutes(5) });
    private MemoryCacheEntryOptions EntryOptions { get; set; } = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12) };

    public bool TryGet(TKey key, out TValue value) =>
        Cache.TryGetValue(key, out value);

    public TValue GetOrCreate(TKey key, Func<TValue> factory) =>
        Cache.GetOrCreate(key, _ => factory(), EntryOptions);

    public void Remove(TKey key) =>
        Cache.Remove(key);

    public void Set(TKey key, TValue value) =>
        Cache.Set(key, value, EntryOptions);

    protected virtual void SetEntryOptions(MemoryCacheEntryOptions options) =>
        EntryOptions = options;

}
