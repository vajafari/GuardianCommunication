using System;
using System.Collections.Generic;

namespace GuardianCommunication.Business.Cache
{

	public interface ICacheManager<T> : IDisposable where T : class
	{

		void AddItem(T value);

		void AddItem(string key, T value);

		T GetCacheItem(string key);

		bool Remove(string key);
		
		bool Contain(string key);

		void Flush();

		IEnumerable<string> GetAllKeys();

		IEnumerable<T> GetAllItems();

		IEnumerable<T> Filter(Func<T, bool> expression);

		
		long Count();

	}

}
