using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.SearchDataWrapper;
using GuardianCommunication.Shared.SharedSettings;
using System;
using System.Linq;
using System.Threading;

namespace GuardianCommunication.Business.Tasks
{
    public class AutoCollectTimyTask : TimedBaseTask
    {
        private readonly DeviceComponent _deviceComponent;
        private readonly CommunicationComponent _communicationComponent;

        public AutoCollectTimyTask(TimeSpan interval) : base(interval)
        {
            var repositoryFactory = new RepositoryFactory();
            _deviceComponent = new DeviceComponent(repositoryFactory);
            _communicationComponent = new CommunicationComponent(repositoryFactory);
            LoggingSystem.LogInfo($"AutoCollectTimyTask started with interval {(int)interval.TotalMinutes}");
        }

        private readonly object _lockCollect = new object();
        public override void Process()
        {
            if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.AutoCollect))
            {
                LoggingSystem.LogInfo("AutoCollectTimyTask process is calling");
            }
            if (Monitor.TryEnter(_lockCollect))
            {
                try
                {
                    var allDeviceInCache = _deviceComponent.SearchDevice(
                            new PagingData<DeviceFilter, DeviceSortEnumeration>())
                        .Where(row => row.ProducerNumber == ProducerEnumeration.Timy).ToList();

                    foreach (var device in allDeviceInCache)
                    {
                        try
                        {
                            if (device.DeviceSettings != null
                                && device.DeviceSettings.IsAutomaticDataCollectActive
                                && !device.DeviceSettings.DontSaveAttendance)
                            {
                                if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.AutoCollect))
                                {
                                    LoggingSystem.LogInfo("AutoCollectSupremaSdk1Task is calling for Timy device", device);
                                }


                                //var oldRecordCount = _communicationComponent.CommunicationRecordCount(deviceInfo);
                                var saveResult = _communicationComponent.DownloadAndSaveUnreadAttendancesFromSdk(device);
                                if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.AutoCollect))
                                {
                                    LoggingSystem.LogInfo("AutoCollectSupremaSdk1Task Result Download and save Timy attendance", saveResult);
                                }
                                //if (AppConfigs.LogLevel.HasFlag(LogLevelEnumeration.LogAutoCollectTimer))
                                //{
                                //    LoggingSystem.LogInfo("Timy save data result", new
                                //    {
                                //        oldRecordCount,
                                //        saveResult
                                //    });
                                //}

                                if (saveResult != null && saveResult.UnknownErrorSave.IsCollectionNotNullOrEmpty())
                                {
                                    LoggingSystem.LogInfo("AutoCollectSupremaSdk1Task Cannot clear data because of save result", ObjectHelper.SerializeAsJson(saveResult));
                                }

                                //if (saveResult != null && saveResult.UnknownErrorSave.IsCollectionNullOrEmpty())
                                //{
                                //    var currentRecordCount = _communicationComponent.CommunicationRecordCount(deviceInfo);
                                //    if (oldRecordCount == currentRecordCount)
                                //    {
                                //        _communicationComponent.CommunicationClearData(deviceInfo);
                                //        if (AppConfigs.LogLevel.HasFlag(LogLevelEnumeration.LogAutoCollectTimer))
                                //        {
                                //            LoggingSystem.LogInfo("Timy clear data is called");
                                //        }
                                //    }
                                //    else
                                //    {
                                //        if (AppConfigs.LogLevel.HasFlag(LogLevelEnumeration.LogAutoCollectTimer))
                                //        {
                                //            LoggingSystem.LogInfo("Timy, Cannot clear data because record count", new { oldRecordCount, currentRecordCount });
                                //        }
                                //    }
                                //}
                                //else
                                //{
                                //    if (AppConfigs.LogLevel.HasFlag(LogLevelEnumeration.LogAutoCollectTimer))
                                //    {
                                //        LoggingSystem.LogInfo("Timy, Cannot clear data because of save result", ObjectHelper.SerializeAsJson(saveResult));
                                //    }
                                //}

                            }

                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "AutoCollectSupremaSdk1Task Error on AutoCollectTimyTask", device);
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
                if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.AutoCollect))
                {
                    LoggingSystem.LogInfo("AutoCollectSupremaSdk1Task skipped because of lock is taken");
                }
            }
        }

    }
}
