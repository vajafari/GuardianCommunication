using GuardianCommunication.Business.Cache;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.SearchDataWrapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuardianCommunication.Business.Component
{
    public class DeviceComponent : BaseComponent
    {

        public DeviceComponent(RepositoryFactory repositoryFactory) : base(repositoryFactory)
        { }


        #region Device

        public List<DtoDevice> SearchDevice(PagingData<DeviceFilter, DeviceSortEnumeration> searchInfo)
        {
            return RepositoryFactory.GetDeviceRepository()
                .SearchFullInfo(searchInfo);
        }

        public DtoDevice GetDeviceCache(Guid deviceId)
        {
            return GuardianCommunicationInMemoryCacheWrapper.Instance.GetDeviceById(deviceId);
        }

        #endregion


        #region DeviceDoor

        public List<DtoDeviceDoorFullInfo> SearchDeviceDoor(PagingData<DeviceDoorFilter, DeviceDoorSortEnumeration> searchInfo)
        {
            return RepositoryFactory.GetDeviceDoorRepository()
                .Search(searchInfo);
        }

        public DtoDeviceDoorFullInfo GetDeviceDoorCache(Guid deviceDoorId)
        {
            return GuardianCommunicationInMemoryCacheWrapper.Instance.GetDeviceDoorById(deviceDoorId);
        }

        #endregion



        #region Device Communication Data

        internal void SaveDeviceCommunicationDataInfo(DtoDeviceCommunicationData entity)
        {
            var deviceInDatabase = GetDeviceCommunicationDataByDeviceIds
                (new List<Guid> { entity.DeviceId }).FirstOrDefault();
            if (deviceInDatabase != null)
            {
                RepositoryFactory.GetDeviceCommunicationDataRepository().Update(entity);
            }
            else
            {
                RepositoryFactory.GetDeviceCommunicationDataRepository().Insert(entity);
            }
        }

        internal List<DtoDeviceCommunicationData> GetDeviceCommunicationDataByDeviceIds(List<Guid> deviceIds)
        {
            return RepositoryFactory.GetDeviceCommunicationDataRepository().GetByDeviceIds(deviceIds);
        }

        #endregion

    }
}
