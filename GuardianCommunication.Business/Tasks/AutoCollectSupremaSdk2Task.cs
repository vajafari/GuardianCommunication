using System;
using System.Linq;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Hardware.Shared;
using GuardianCommunication.Hardware.Suprema.SupremaConcepts.V2;

namespace GuardianCommunication.Business.Tasks
{
    public class AutoCollectSupremaSdk2Task : TimedBaseTask
    {
        private readonly DeviceComponent _deviceComponent;
        private readonly CommunicationComponent _communicationComponent;

        public AutoCollectSupremaSdk2Task(TimeSpan interval) : base(interval)
        {
            var repositoryFactory = new RepositoryFactory();
            _deviceComponent = new DeviceComponent(repositoryFactory);
            _communicationComponent = new CommunicationComponent(repositoryFactory);
            LoggingSystem.LogInfo($"AutoCollectSupremaSdk2Task started with interval {(int)interval.TotalMinutes}");
        }

        private readonly object _lockCollect = new object();
        public override void Process()
        {
            if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.AutoCollect))
            {
                LoggingSystem.LogInfo("AutoCollectSupremaSdk2Task process is calling");
            }
            if (Monitor.TryEnter(_lockCollect))
            {
                try
                {

                    var allDeviceInCache = _deviceComponent.SearchDeviceCache(row => true)
                        .Where(row => row.DeviceTypeSummary.ProducerNumber == ProducerEnumeration.Suprema
                        && row.DeviceTypeSummary.SdkVersion == SdkVersionEnumeration.SdkVersion2).ToList();

                    foreach (var device in allDeviceInCache)
                    {
                        var deviceInfo = _deviceComponent.ConvertDeviceToDeviceInfo(device);
                        try
                        {
                            if (device.AutomaticDataCollect && !device.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
                            {
                                if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.AutoCollect))
                                {
                                    LoggingSystem.LogInfo("AutoCollectSupremaSdk2Task is calling for Suprema SDK 2 device",
                                        device);
                                }
                                var result = _communicationComponent.DownloadAndSaveUnreadAttendancesFromSdk(deviceInfo);
                                if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.AutoCollect))
                                {
                                    LoggingSystem.LogInfo("AutoCollectSupremaSdk2Task Result Download and save Suprema SDK 2 attendance", result);
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "AutoCollectSupremaSdk2Task Error on AutoCollectSupremaSdk2Task attendance collect", device);
                        }


                        try
                        {
                            if (device.DeviceSettings.HasFlag(DeviceSettingsEnumeration.SupremaAutoCollectEvents)
                                && !device.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveEvents))
                            {
                                var logs = _communicationComponent.CommunicationGetUnreadLogs(deviceInfo);
                                if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.AutoCollect))
                                {
                                    LoggingSystem.LogInfo("AutoCollectSupremaSdk2Task Result Auto collect logs for suprema 2", logs);
                                }
                                foreach (var currentLog in logs)
                                {
                                    var eventType = SupremaV2Utility.GetEventType(currentLog.EventCode);
                                    if (eventType == SupremaSdk2EventTypeEnumeration.UserChanged
                                        && device.IsMasterDevice
                                        && currentLog.IsFromDevice
                                        && currentLog.EmployeeNumber.HasValue)
                                    {
                                        HardwareEventPublisher.Instance.PublishUserChangedReceived(
                                            deviceInfo.DeviceNumber, currentLog.EmployeeNumber.Value);
                                    }

                                    if (eventType == SupremaSdk2EventTypeEnumeration.OtherEvents)
                                    {
                                        HardwareEventPublisher.Instance.PublishDeviceEventLogData(currentLog);
                                    }
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "AutoCollectSupremaSdk2Task Error on AutoCollectSupremaSdk2Task event log collect", device);
                        }

                    }

                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "AutoCollectSupremaSdk2Task Error on AutoCollectSupremaSdk2Task collect");
                }
                finally
                {
                    Monitor.Exit(_lockCollect);
                }
            }
            else
            {
                if (AppConfigs.LogLevelSuprema2.HasFlag(LogLevelSuprema2Enumeration.AutoCollect))
                {
                    LoggingSystem.LogInfo("AutoCollectSupremaSdk2Task skipped because of lock is taken");
                }
            }
        }
    }
}
