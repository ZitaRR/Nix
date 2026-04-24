using System;

namespace Nix.Infrastructure.Memory;

public interface ICache<TKey, TValue>
{
    bool TryGet(TKey key, out TValue value);
    TValue GetOrCreate(TKey key, Func<TValue> factory);
    void Set(TKey key, TValue value);
    void Remove(TKey key);
}
