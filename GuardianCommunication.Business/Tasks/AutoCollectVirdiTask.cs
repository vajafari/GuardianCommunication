using System;
using System.Linq;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;

namespace GuardianCommunication.Business.Tasks
{
    public class AutoCollectVirdiTask : TimedBaseTask
    {
        private readonly DeviceComponent _deviceComponent;
        private readonly CommunicationComponent _communicationComponent;

        public AutoCollectVirdiTask(TimeSpan interval) : base(interval)
        {
            var repositoryFactory = new RepositoryFactory();
            _deviceComponent = new DeviceComponent(repositoryFactory);
            _communicationComponent = new CommunicationComponent(repositoryFactory);
            LoggingSystem.LogInfo($"AutoCollectVirdiTask started with interval {(int)interval.TotalMinutes}");
        }

        private readonly object _lockCollect = new object();
        public override void Process()
        {
            if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.AutoCollect))
            {
                LoggingSystem.LogInfo("AutoCollectVirdiTask process is calling");
            }
            if (Monitor.TryEnter(_lockCollect))
            {
                try
                {

                    var allDeviceInCache = _deviceComponent.SearchDeviceCache(row => true)
                        .Where(row => row.DeviceTypeSummary.ProducerNumber == ProducerEnumeration.Virdi)
                        // به دلیل اینکه در سیستم های پارکینگ می بایست تردد های
                        // ورود ابتدا جمع شوند این مرتب سازی را انجام دادیم
                        .OrderBy(d => d.IoType)
                        .ToList();

                    foreach (var device in allDeviceInCache)
                    {
                        try
                        {
                            if (device.AutomaticDataCollect && !device.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
                            {
                                if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.AutoCollect))
                                {
                                    LoggingSystem.LogInfo("AutoCollectVirdiTask is calling for Virdi device", device);
                                }
                                // به دلیل اینکه ویردی به هیچ عنوان دستورات سینک نداره 
                                // به همین دیلی نتیجه جمع آوری برای ما مهم نیست
                                var deviceInfo = _deviceComponent.ConvertDeviceToDeviceInfo(device);
                                //var oldRecordCount = _communicationComponent.CommunicationRecordCount(deviceInfo);
                                var result = _communicationComponent.DownloadAndSaveUnreadAttendancesFromSdk(deviceInfo);
                                if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.AutoCollect))
                                {
                                    LoggingSystem.LogInfo("AutoCollectVirdiTask Result Download and save Virdi attendance", result);
                                }

                            }
                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "AutoCollectVirdiTask Error on AutoCollectVirdiTask", device);
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
                if (AppConfigs.LogLevelVirdi.HasFlag(LogLevelVirdiEnumeration.AutoCollect))
                {
                    LoggingSystem.LogInfo("AutoCollectVirdiTask skipped because of lock is taken");
                }
            }
        }
    }
}
