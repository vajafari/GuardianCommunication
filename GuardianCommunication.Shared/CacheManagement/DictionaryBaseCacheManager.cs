using System;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.CacheManagement;

public class DictionaryBaseCacheManager<T> : ICacheManager<T> where T : class
{
    protected const string KeyIsNotValid = "Key is not valid";
    protected const string ValueIsNotValid = "Value is not valid";

    protected Dictionary<string, T> CachedItem = new();

    public void AddItem(T value)
    {
 
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value), ValueIsNotValid);
        }

        if (!(value is ICacheable valueAsCachable))
        {
            throw new InvalidProgramException("Value is not implement ICacheable. Key must be provided for add");
        }

        AddItem(valueAsCachable.GetKey(), value);

    }

    public void AddItem(string key, T value)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key), KeyIsNotValid);
        }

        if (value == null)
        {
            throw new ArgumentNullException(nameof(value), ValueIsNotValid);
        }


        lock (CachedItem)
        {
            CachedItem[key] = value;
        }

    }

    public T GetCacheItem(string key)
    {
        T result = null;
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key), KeyIsNotValid);
        }

        lock (CachedItem)
        {
            if (CachedItem.TryGetValue(key, out var value))
            {
                result = value;
            }
        }
        return result;
    }

    public bool Remove(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException(nameof(key), KeyIsNotValid);
        }
        lock (CachedItem)
        {
            return CachedItem.Remove(key);
        }

    }
		
}