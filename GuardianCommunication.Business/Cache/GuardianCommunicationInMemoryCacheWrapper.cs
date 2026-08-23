using System;
using System.Collections.Generic;
using System.Linq;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Shared.CacheManagement;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.SearchDataWrapper;

namespace GuardianCommunication.Business.Cache
{
    public class GuardianCommunicationInMemoryCacheWrapper : IGuardianCommunicationCacheManager
    {

        #region Singleton

        public static GuardianCommunicationInMemoryCacheWrapper Instance { get; }

        static GuardianCommunicationInMemoryCacheWrapper()
        {
            Instance = new GuardianCommunicationInMemoryCacheWrapper();
        }

        private GuardianCommunicationInMemoryCacheWrapper()
        {
        }

        #endregion

        // Private cache managers
        private ICacheManager<DtoDevice> DeviceManager { get; } = new DictionaryBaseCacheManager<DtoDevice>();
        private ICacheManager<DtoDeviceDoorFullInfo> DeviceDoorManager { get; } = new DictionaryBaseCacheManager<DtoDeviceDoorFullInfo>();
        private List<DtoHookDefinition> _hookDefinitions = null;
        private DtoSystemConfig _systemConfig;
        
        
        // Per-entity-type fetch locks: prevent duplicate DB reads on concurrent cache misses
        private readonly object _deviceFetchLock = new object();
        private readonly object _deviceDoorFetchLock = new object();
        private readonly object _hookDefinitionFetchLock = new object();
        
        #region Device

        public DtoDevice GetDeviceById(Guid deviceId)
        {
            var cached = DeviceManager.GetCacheItem(deviceId.ToString());
            if (cached != null) return cached;

            lock (_deviceFetchLock)
            {

                cached = DeviceManager.GetCacheItem(deviceId.ToString());
                if (cached != null) return cached;
                var repositoryFactory = new RepositoryFactory();
                var item = (repositoryFactory.GetDeviceRepository().SearchFullInfo
                    (new PagingData<DeviceFilter, DeviceSortEnumeration>
                    {
                        Filter = new DeviceFilter
                        {
                            Ids = new List<Guid>() { deviceId }
                        }
                    })).FirstOrDefault();
                if (item != null)
                {
                    DeviceManager.AddItem(item.Id.ToString(), item);
                    return item;
                }
                return null;
            }
        }

        #endregion

        #region DeviceDoor

        public DtoDeviceDoorFullInfo GetDeviceDoorById(Guid deviceDoorId)
        {
            var cached = DeviceDoorManager.GetCacheItem(deviceDoorId.ToString());
            if (cached != null) return cached;

            lock (_deviceDoorFetchLock)
            {

                cached = DeviceDoorManager.GetCacheItem(deviceDoorId.ToString());
                if (cached != null) return cached;
                var repositoryFactory = new RepositoryFactory();
                var item = (repositoryFactory.GetDeviceDoorRepository().Search
                (new PagingData<DeviceDoorFilter, DeviceDoorSortEnumeration>
                {
                    Filter = new DeviceDoorFilter
                    {
                        Ids = new List<Guid>() { deviceDoorId }
                    }
                })).FirstOrDefault();
                if (item != null)
                {
                    DeviceDoorManager.AddItem(item.Id.ToString(), item);
                    return item;
                }
                return null;
            }
        }

        #endregion

        #region HookDefinition

        public List<DtoHookDefinition> GetHookDefinitions()
        {
            if (_hookDefinitions != null)
            {
                return _hookDefinitions;
            }
            lock (_hookDefinitionFetchLock)
            {
                var repositoryFactory = new RepositoryFactory();
                _hookDefinitions = repositoryFactory.GetHookDefinitionRepository().Search
                    (new PagingData<HookDefinitionFilter, HookDefinitionSortEnumeration>()) 
                           ?? new List<DtoHookDefinition>();
            }

            return _hookDefinitions;
        }

        #endregion

        #region System Config


        public DtoSystemConfig GetSystemConfigCache()
        {
            if (_systemConfig != null)
            {
                return _systemConfig;
            }

            return null;
        }

        public void ResetSystemConfigCache()
        {
            _systemConfig = null;
        }

        

        #endregion
    }
}
