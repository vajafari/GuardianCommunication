namespace GuardianCommunication.Data.Repository
{

    public class RepositoryFactory
    {

        public IEndPointUrlRepository GetEndPointUrlRepository()
        {
            return new SqlLiteEndPointUrlRepository(AppConfigs.ConnectionConfig);
        }

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

        public IOtherHardwareCommandRepository GetOtherHardwareCommandRepository()
        {
            return new OtherHardwareCommandRepository(AppConfigs.ConnectionConfig);
        }

        public IHookSystemRepository GetHookSystemRepository()
        {
            return new HookSystemRepository(AppConfigs.ConnectionConfig);
        }

        public IHookSystemDetailRepository GetHookSystemDetailRepository()
        {
            return new HookSystemDetailRepository(AppConfigs.ConnectionConfig);
        }

        public IAttendanceHookSystemRepository GetAttendanceHookSystemRepository()
        {
            return new AttendanceHookSystemRepository(AppConfigs.ConnectionConfig);
        }

        public IAttendanceSendToKarnamaResultRepository GetAttendanceSendToKarnamaResultRepository()
        {
            return new AttendanceSendToKarnamaResultRepository(AppConfigs.ConnectionConfig);
        }

        public IDeviceCommunicationDataRepository GetDeviceCommunicationDataRepository()
        {
            return new DeviceCommunicationDataRepository(AppConfigs.ConnectionConfig);
        }

        public ICameraCommunicationDataRepository GetCameraCommunicationDataRepository()
        {
            return new CameraCommunicationDataRepository(AppConfigs.ConnectionConfig);
        }

        public IScheduledApiCallTaskRepository GetScheduledApiCallTaskRepository()
        {
            return new ScheduledApiCallTaskRepository(AppConfigs.ConnectionConfig);
        }

        public ILogRepository GetLogRepository()
        {
            return new LogRepository(AppConfigs.ConnectionConfig);
        }

    }

}
