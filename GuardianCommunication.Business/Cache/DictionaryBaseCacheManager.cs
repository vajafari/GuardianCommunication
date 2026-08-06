using System;
using System.Collections.Generic;
using System.Linq;

namespace GuardianCommunication.Business.Cache
{
    public class DictionaryBaseCacheManager<T> : ICacheManager<T> where T : class
    {
        protected const string KeyIsNotValid = "Key is not valid";
        protected const string ValueIsNotValid = "Value is not valid";

        protected Dictionary<string, T> CachedItem = new Dictionary<string, T>();

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
                if (!CachedItem.ContainsKey(key))
                {
                    CachedItem.Add(key, value);
                }
                else
                {
                    CachedItem[key] = value;
                }
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

        public bool Contain(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key), KeyIsNotValid);
            }

            lock (CachedItem)
            {
                return CachedItem.ContainsKey(key);
            }
        }

        public void Flush()
        {

            lock (CachedItem)
            {
                var allKeys = CachedItem.Keys.ToList();
                foreach (var key in allKeys)
                {
                    Remove(key);
                }
            }

            GC.Collect();
        }

        public IEnumerable<string> GetAllKeys()
        {
            lock (CachedItem)
            {
                return CachedItem.Keys;
            }
        }

        public IEnumerable<T> GetAllItems()
        {
            lock (CachedItem)
            {
                return CachedItem.Values;
            }
        }

        public IEnumerable<T> Filter(Func<T, bool> expression)
        {
            lock (CachedItem)
            {
                return CachedItem.Values.Where(expression);
            }

        }


        public long Count()
        {
            lock (CachedItem)
            {
                return CachedItem.Count;
            }
        }




        #region Implementation of IDisposable

        /// <summary>
        ///   Releases all resources used by the WarrantManagement.DataExtract.Dal.ReportDataBase
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases all resources used by the WarrantManagement.DataExtract.Dal.ReportDataBase
        /// </summary>
        /// <param name="disposing"> A boolean value indicating whether or not to dispose managed resources </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (CachedItem == null) return;
            lock (CachedItem)
            {
                CachedItem.Clear();
                CachedItem = null;
            }

        }

        #endregion






    }
}
