using System;
using System.Linq;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;

namespace GuardianCommunication.Business.Tasks
{
    public class AutoCollectElmoSanatTask : TimedBaseTask
    {
        private readonly DeviceComponent _deviceComponent;
        private readonly CommunicationComponent _communicationComponent;

        public AutoCollectElmoSanatTask(TimeSpan interval) : base(interval)
        {
            var repositoryFactory = new RepositoryFactory();
            _deviceComponent = new DeviceComponent(repositoryFactory);
            _communicationComponent = new CommunicationComponent(repositoryFactory);
            LoggingSystem.LogInfo($"AutoCollectElmoSanatTask started with interval {(int)interval.TotalMinutes}");
        }

        private readonly object _lockCollect = new object();
        public override void Process()
        {
            if (AppConfigs.LogLevelElmoSanat.HasFlag(LogLevelElmoSanatEnumeration.AutoCollect))
            {
                LoggingSystem.LogInfo("AutoCollectElmoSanatTask is calling");
            }
            if (Monitor.TryEnter(_lockCollect))
            {
                try
                {

                    var allDeviceInCache = _deviceComponent.SearchDeviceCache(row => true)
                     .Where(row => row.DeviceTypeSummary.ProducerNumber == ProducerEnumeration.ElmOSanat).ToList();
                    foreach (var device in allDeviceInCache)
                    {
                        try
                        {
                            if (device.AutomaticDataCollect && !device.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
                            {
                                if (AppConfigs.LogLevelElmoSanat.HasFlag(LogLevelElmoSanatEnumeration.AutoCollect))
                                {
                                    LoggingSystem.LogInfo("AutoCollectElmoSanatTask is calling for  ElmoSanat device", device);
                                }
                                var deviceInfo = _deviceComponent.ConvertDeviceToDeviceInfo(device);
                                var saveResult = _communicationComponent.DownloadAndSaveUnreadAttendancesFromSdk(deviceInfo);
                                if (AppConfigs.LogLevelElmoSanat.HasFlag(LogLevelElmoSanatEnumeration.AutoCollect))
                                {
                                    LoggingSystem.LogInfo("AutoCollectElmoSanatTask save data result", saveResult);
                                }
                            }

                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "AutoCollectElmoSanatTask Error on AutoCollectElmoSanatTask", device);
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(_lockCollect);
                }
            }
            else
            {
                if (AppConfigs.LogLevelElmoSanat.HasFlag(LogLevelElmoSanatEnumeration.AutoCollect))
                {
                    LoggingSystem.LogInfo("AutoCollectElmoSanatTask skipped because of lock is taken");
                }
            }
        }
    }
}
