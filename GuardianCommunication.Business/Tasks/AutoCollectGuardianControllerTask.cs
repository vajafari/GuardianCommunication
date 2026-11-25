using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.SearchDataWrapper;
using GuardianCommunication.Shared.SharedSettings;
using System;
using System.Linq;
using System.Threading;

namespace GuardianCommunication.Business.Tasks
{
    /// <summary>
    /// خواندن ترددهای بردهای کنترلر گاردین در حالت Standalone؛ در Push برد خودش ترددها را می‌فرستد
    /// </summary>
    public class AutoCollectGuardianControllerTask : TimedBaseTask
    {
        private readonly DeviceComponent _deviceComponent;
        private readonly CommunicationComponent _communicationComponent;

        public AutoCollectGuardianControllerTask(TimeSpan interval) : base(interval)
        {
            var repositoryFactory = new RepositoryFactory();
            _deviceComponent = new DeviceComponent(repositoryFactory);
            _communicationComponent = new CommunicationComponent(repositoryFactory);
            LoggingSystem.LogInfo($"AutoCollectGuardianControllerTask started with interval {(int)interval.TotalMinutes}");
        }

        private readonly object _lockCollect = new object();

        public override void Process()
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.ReadLog))
            {
                LoggingSystem.LogInfo("AutoCollectGuardianControllerTask process is calling");
            }
            if (!Monitor.TryEnter(_lockCollect))
            {
                if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.ReadLog))
                {
                    LoggingSystem.LogInfo("AutoCollectGuardianControllerTask skipped because of lock is taken");
                }
                return;
            }
            try
            {
                var standaloneBoards = _deviceComponent.SearchDevice(new PagingData<DeviceFilter, DeviceSortEnumeration>())
                    .Where(device => device.ProducerNumber == ProducerEnumeration.Guardian
                                     && device.ConnectionMode == DeviceConnectionModeEnumeration.Standalone
                                     && device.IsActive
                                     && device.DeviceSettings != null
                                     && device.DeviceSettings.IsAutomaticDataCollectActive
                                     && !device.DeviceSettings.DontSaveAttendance)
                    .ToList();
                foreach (var device in standaloneBoards)
                {
                    try
                    {
                        if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.ReadLog))
                        {
                            LoggingSystem.LogInfo("AutoCollectGuardianControllerTask is calling for guardian controller device", new { device.Id });
                        }
                        var result = _communicationComponent.DownloadAndSaveUnreadAttendancesFromSdk(device);
                        if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.ReadLog))
                        {
                            LoggingSystem.LogInfo("AutoCollectGuardianControllerTask Result Download and save guardian controller attendance", result);
                        }
                    }
                    catch (Exception exp)
                    {
                        // برد در دسترس نبود یا خطا داد؛ دور بعد از همان زمان آخرین تردد خوانده‌شده دوباره خوانده می‌شود
                        LoggingSystem.LogError(exp, "AutoCollectGuardianControllerTask Error on attendance collect", new { device.Id, device.DeviceIp });
                    }
                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "AutoCollectGuardianControllerTask Error on collect");
            }
            finally
            {
                Monitor.Exit(_lockCollect);
            }
        }
    }
}
