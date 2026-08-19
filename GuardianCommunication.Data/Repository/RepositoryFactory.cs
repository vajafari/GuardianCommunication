using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Data.Repository
{

    public class RepositoryFactory
    {

        public ISystemConfigRepository GetSystemConfigRepository()
        {
            return new SystemConfigRepository(AppConfigs.ConnectionConfig);
        }

        public IAttendanceRepository GetAttendanceRepository()
        {
            return new AttendanceRepository(AppConfigs.ConnectionConfig);
        }

        public IDeviceCommandRepository GetDeviceCommandRepository()
        {
            return new DeviceCommandRepository(AppConfigs.ConnectionConfig);
        }

        public IDeviceCommunicationDataRepository GetDeviceCommunicationDataRepository()
        {
            return new DeviceCommunicationDataRepository(AppConfigs.ConnectionConfig);
        }

        public ILogRepository GetLogRepository()
        {
            return new LogRepository(AppConfigs.ConnectionConfig);
        }

        public IAttendanceHookDefinitionRepository GetAttendanceHookDefinitionRepository()
        {
            return new AttendanceHookDefinitionRepository(AppConfigs.ConnectionConfig);
        }

        public IHookDefinitionRepository GetHookDefinitionRepository()
        {
            return new HookDefinitionRepository(AppConfigs.ConnectionConfig);
        }

        public IDeviceDoorRepository GetDeviceDoorRepository()
        {
            return new DeviceDoorRepository(AppConfigs.ConnectionConfig);
        }

        public IDeviceRepository GetDeviceRepository()
        {
            return new DeviceRepository(AppConfigs.ConnectionConfig);
        }

    }

}
