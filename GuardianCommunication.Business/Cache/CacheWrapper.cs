using System.Collections.Generic;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Repository;

namespace GuardianCommunication.Business.Cache
{
	public class CacheWrapper
	{

		private readonly RepositoryFactory _repositoryFactory;
		
		#region Singleton

		public static CacheWrapper Instance { get; }

		static CacheWrapper()
		{
			Instance = new CacheWrapper();
		}

		private CacheWrapper()
		{
			_repositoryFactory = new RepositoryFactory();
			DeviceCacheManager = new DictionaryBaseCacheManager<DtoDevice>();
			MetalDetectorGateCacheManager = new DictionaryBaseCacheManager<DtoMetalDetectorGate>();
			XRayDeviceCacheManager = new DictionaryBaseCacheManager<DtoXRayDevice>();
			CameraCacheManager = new DictionaryBaseCacheManager<DtoCamera>();
			HookSystemCacheManager = new DictionaryBaseCacheManager<DtoHookSystem>();
			DeviceDoorCacheManager = new DictionaryBaseCacheManager<DtoDeviceDoor>();

        }

		#endregion


		#region SystemConfig

		public DtoSystemConfig SystemConfig { get; private set; }

		public void ResetSystemConfigCache()
		{
			var systemConfigComponent = new SystemConfigComponent(_repositoryFactory);
			SystemConfig = systemConfigComponent.GetSystemConfig();
#if DEBUG
            SystemConfig.KarnamaServiceUrl = "https://localhost:44317/";
#endif

        }

		#endregion


		#region Device

		public ICacheManager<DtoDevice> DeviceCacheManager { get; }

		public void ResetDeviceCache()
		{
			DeviceCacheManager.Flush();
			var deviceComponent = new DeviceComponent(_repositoryFactory);
			SetDeviceCache(deviceComponent.GetDeviceList());
		}


		public void SetDeviceCache(List<DtoDevice> deviceList)
		{
			DeviceCacheManager.Flush();
			if (deviceList.IsCollectionNullOrEmpty()) return;
			foreach (var device in deviceList)
			{
				DeviceCacheManager.AddItem(device.DeviceNumber.ToString(), device);
			}
		}


        #endregion

        
		#region Device Door

        public ICacheManager<DtoDeviceDoor> DeviceDoorCacheManager { get; }

        public void ResetDeviceDoorCache()
        {
            DeviceDoorCacheManager.Flush();
            var deviceComponent = new DeviceComponent(_repositoryFactory);
            SetDeviceDoorCache(deviceComponent.GetDeviceDoorList());
        }

        public void SetDeviceDoorCache(List<DtoDeviceDoor> controllerDeviceDoorList)
        {
            DeviceDoorCacheManager.Flush();
            if (controllerDeviceDoorList.IsCollectionNullOrEmpty()) return;
            foreach (var door in controllerDeviceDoorList)
            {
                DeviceDoorCacheManager.AddItem(door.Id.ToString(), door);
            }
        }


        #endregion


        #region MetalDetectorGate

        public ICacheManager<DtoMetalDetectorGate> MetalDetectorGateCacheManager { get; }

        public void ResetMetalDetectorGateCache()
        {
            MetalDetectorGateCacheManager.Flush();
            var deviceComponent = new DeviceComponent(_repositoryFactory);
            SetMetalDetectorGateCache(deviceComponent.FetchAllActiveMetalDetectorGates());
        }


        public void SetMetalDetectorGateCache(List<DtoMetalDetectorGate> metalDetectorGateList)
        {
            MetalDetectorGateCacheManager.Flush();
            if (!metalDetectorGateList.IsCollectionNotNullOrEmpty()) return;
            foreach (var item in metalDetectorGateList)
            {
                MetalDetectorGateCacheManager.AddItem(item.Id.ToString(), item);
            }

        }


        #endregion


        #region XRayDevice

        public ICacheManager<DtoXRayDevice> XRayDeviceCacheManager { get; }

        public void ResetXRayDeviceCache()
        {
            XRayDeviceCacheManager.Flush();
            var deviceComponent = new DeviceComponent(_repositoryFactory);
            SetXRayDeviceCache(deviceComponent.FetchAllActiveXRayDevices());
        }


        public void SetXRayDeviceCache(List<DtoXRayDevice> xRayDeviceList)
        {
            XRayDeviceCacheManager.Flush();
            if (!xRayDeviceList.IsCollectionNotNullOrEmpty()) return;
            foreach (var item in xRayDeviceList)
            {
                XRayDeviceCacheManager.AddItem(item.Id.ToString(), item);
            }

        }


        #endregion


        #region Camera

        public ICacheManager<DtoCamera> CameraCacheManager { get; }

        public void ResetCameraCache()
        {
            CameraCacheManager.Flush();
            var cameraComponent = new CameraComponent(_repositoryFactory);
            SetCameraCache(cameraComponent.FetchAllActiveCameras());
        }


        public void SetCameraCache(List<DtoCamera> xRayDeviceList)
        {
            CameraCacheManager.Flush();
            if (!xRayDeviceList.IsCollectionNotNullOrEmpty()) return;
            foreach (var item in xRayDeviceList)
            {
                CameraCacheManager.AddItem(item.Id.ToString(), item);
            }

        }


        #endregion


        #region HookSystemCache


        public ICacheManager<DtoHookSystem> HookSystemCacheManager { get; }

		public void ResetHookSystemCache()
		{
			HookSystemCacheManager.Flush();
			var hookComponent = new HookComponent(_repositoryFactory);
			var hookSystemsList = hookComponent.SearchHookSystem(null, true);

			if (!hookSystemsList.IsCollectionNotNullOrEmpty()) return;
			foreach (var hookSystemCache in hookSystemsList)
			{
				HookSystemCacheManager.AddItem(hookSystemCache.Id.ToString(), hookSystemCache);
			}
		}

		public void ResetHookSystemCache(List<int> ids)
		{
			HookSystemCacheManager.Flush();
			var hookComponent = new HookComponent(_repositoryFactory);
			var hookSystemsList = hookComponent.SearchHookSystem(
				new PagingData<HookSystemFilter, HookSystemSortEnumeration>
				{
					Filter = new HookSystemFilter
					{
						Ids = ids
					}
				}, true);

			if (!hookSystemsList.IsCollectionNotNullOrEmpty()) return;
			foreach (var hookSystemCache in hookSystemsList)
			{
				HookSystemCacheManager.AddItem(hookSystemCache.Id.ToString(), hookSystemCache);
			}
		}

		#endregion


		public void ResetAllCaches()
		{
			ResetSystemConfigCache();
			ResetHookSystemCache();
			ResetDeviceCache();
			ResetDeviceDoorCache();
			ResetMetalDetectorGateCache();
			ResetXRayDeviceCache();
			ResetCameraCache();
			//try
			//{
			//	ResetCameraCache();
			//}
			//catch (Exception exp)
			//{
			//	LoggingSystem.LogError(exp);
			//}
		}

	}



}
