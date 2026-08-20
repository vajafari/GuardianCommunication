using System;
using GuardianCommunication.Shared.Dto;

namespace GuardianCommunication.Business.Cache
{
    public interface IGuardianCommunicationCacheManager
    {
       
        #region Device

        DtoDevice GetDeviceById(Guid deviceId);

        #endregion

        #region DeviceDoor

        DtoDeviceDoorFullInfo GetDeviceDoorById(Guid deviceDoor);

        #endregion

        #region SystemConfig

        DtoSystemConfig GetSystemConfigCache();

        #endregion

    }
}
