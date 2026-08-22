using System;
using System.Linq;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Hardware.Shared;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.SearchDataWrapper;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Business.Tasks
{
    public class AutoCollectSupremaSdk1Task : TimedBaseTask
    {
        private readonly DeviceComponent _deviceComponent;
        private readonly CommunicationComponent _communicationComponent;

        public AutoCollectSupremaSdk1Task(TimeSpan interval) : base(interval)
        {
            var repositoryFactory = new RepositoryFactory();
            _deviceComponent = new DeviceComponent(repositoryFactory);
            _communicationComponent = new CommunicationComponent(repositoryFactory);
            LoggingSystem.LogInfo($"AutoCollectSupremaSdk1Task started with interval {(int)interval.TotalMinutes}");
        }

        private readonly object _lockCollect = new object();
        public override void Process()
        {
            if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.AutoCollect))
            {
                LoggingSystem.LogInfo("AutoCollectSupremaSdk1Task process is calling");
            }
            if (Monitor.TryEnter(_lockCollect))
            {
                try
                {

                    var allDeviceInCache = _deviceComponent.SearchDevice(
                            new PagingData<DeviceFilter, DeviceSortEnumeration>())
                        .Where(row => row.ProducerNumber == ProducerEnumeration.Suprema
                        && row.SdkVersion == SdkVersionEnumeration.SdkVersion1).ToList();

                    foreach (var device in allDeviceInCache)
                    {
                        try
                        {
                            if (device.DeviceSettings != null 
                                && device.DeviceSettings.IsAutomaticDataCollectActive 
                                && !device.DeviceSettings.DontSaveAttendance)
                            {
                                if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.AutoCollect))
                                {
                                    LoggingSystem.LogInfo("AutoCollectSupremaSdk1Task is calling for Suprema SDK 1 device", device);
                                }
                                var result = _communicationComponent
                                    .DownloadAndSaveUnreadAttendancesFromSdk(device);
                                if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.AutoCollect))
                                {
                                    LoggingSystem.LogInfo("AutoCollectSupremaSdk1Task Result Download and save Suprema SDK 1 attendance", result);
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "AutoCollectSupremaSdk1Task Error on AutoCollectSupremaSdk1Task attendance collect", device);
                        }

                        try
                        {
                            if (device.DeviceSettings != null
                                && device.DeviceSettings.IsAutomaticDataCollectActive
                                && !device.DeviceSettings.DontSaveEvents)
                            {
                                var logs = _communicationComponent.CommunicationGetUnreadLogs(device.Id);
                                if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.AutoCollect))
                                {
                                    LoggingSystem.LogInfo(
                                        "AutoCollectSupremaSdk1Task Result Auto collect logs for suprema 1", logs);
                                }

                                foreach (var currentLog in logs)
                                {
                                    //var eventType = SupremaV2Utility.GetEventType(currentLog.EventCode);
                                    //if (eventType == SupremaSdk2EventTypeEnumeration.UserChanged
                                    //    && device.IsMasterDevice
                                    //    && currentLog.IsFromDevice
                                    //    && currentLog.EmployeeNumber.HasValue)
                                    //{
                                    //    HardwareEventPublisher.Instance.PublishUserChangedReceived(deviceInfo.DeviceNumber, currentLog.EmployeeNumber.Value);
                                    //}
                                    //else
                                    //{
                                    //    HardwareEventPublisher.Instance.PublishDeviceEventLogData(currentLog);
                                    //}
                                    HardwareEventPublisher.Instance.PublishDeviceEventLogData(currentLog);
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "AutoCollectSupremaSdk1Task Error on AutoCollectSupremaSdk1Task event log collect", device);
                        }

                    }
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "AutoCollectSupremaSdk1Task Error on AutoCollectSupremaSdk1Task collect");
                }
                finally
                {
                    Monitor.Exit(_lockCollect);
                }
            }
            else
            {
                if (AppConfigs.LogLevelSuprema1.HasFlag(LogLevelSuprema1Enumeration.AutoCollect))
                {
                    LoggingSystem.LogInfo("AutoCollectSupremaSdk1Task skipped because of lock is taken");
                }
            }
        }
    }
}
