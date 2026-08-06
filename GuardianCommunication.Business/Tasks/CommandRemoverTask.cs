using System;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;

namespace GuardianCommunication.Business.Tasks
{
    public class DeleteUnsentCommandTask : TimedBaseTask
    {

        private readonly DeviceCommandComponent _deviceCommandComponent;
        private readonly SystemConfigComponent _systemConfigComponent;

        public DeleteUnsentCommandTask(TimeSpan interval) : base(interval)
        {

            var repositoryFactory = new RepositoryFactory();
            _deviceCommandComponent = new DeviceCommandComponent(repositoryFactory);
            _systemConfigComponent = new SystemConfigComponent(repositoryFactory);
            LoggingSystem.LogInfo($"DeleteUnsentCommandTask started with interval {(int)interval.TotalHours}");
        }


        private readonly object _lock = new object();
        public override void Process()
        {
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogDeleteUnsentCommandTask))
            {
                LoggingSystem.LogInfo("DeleteUnsentCommandTask process is calling");
            }
            if (Monitor.TryEnter(_lock))
            {
                try
                {
                    var systemConfig = _systemConfigComponent.GetSystemConfigCache();
                    var startDateTime = DateTime.Now.AddDays(-1 * systemConfig.DeleteUnsentCommandsIntervalInDays);
                    _deviceCommandComponent.DeleteFailedBeforeDate(startDateTime, systemConfig.DeleteUnsentCommandsJustDeleteFailed);
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Exception at DeleteUnsentCommandTask");
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }
            else
            {
                if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.LogDeleteUnsentCommandTask))
                {
                    LoggingSystem.LogInfo("DeleteUnsentCommandTask skipped because of lock is taken");
                }
            }
        }

    }
}
