namespace GuardianCommunication.Shared.CacheManagement;

public interface ICacheManager<T>
{

    void AddItem(T value);

    void AddItem(string key, T value);

    T GetCacheItem(string key);

    bool Remove(string key);
		
}