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

        DtoDeviceDoor GetDeviceDoorById(Guid deviceDoor);

        #endregion

        #region SystemConfig

        DtoSystemConfig GetSystemConfigCache();

        #endregion

    }
}
