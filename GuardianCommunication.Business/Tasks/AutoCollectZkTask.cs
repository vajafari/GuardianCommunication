using System;
using System.Linq;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.SearchDataWrapper;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Business.Tasks
{
    public class AutoCollectZkTask : TimedBaseTask
    {
        private readonly DeviceComponent _deviceComponent;
        private readonly CommunicationComponent _communicationComponent;

        public AutoCollectZkTask(TimeSpan interval) : base(interval)
        {
            var repositoryFactory = new RepositoryFactory();
            _deviceComponent = new DeviceComponent(repositoryFactory);
            _communicationComponent = new CommunicationComponent(repositoryFactory);
            LoggingSystem.LogInfo($"AutoCollectZkTask started with interval {(int)interval.TotalMinutes}");
        }

        private readonly object _lockCollect = new object();
        public override void Process()
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AutoCollect))
            {
                LoggingSystem.LogInfo("AutoCollectZkTask process is calling");
            }
            if (Monitor.TryEnter(_lockCollect))
            {
                try
                {

                    var allDeviceInCache = _deviceComponent.SearchDevice(
                            new PagingData<DeviceFilter, DeviceSortEnumeration>())
                        .Where(row => row.ProducerNumber == ProducerEnumeration.Zk)
                        .ToList();

                    foreach (var device in allDeviceInCache)
                    {
                        try
                        {
                            if (device.DeviceSettings != null
                                && device.DeviceSettings.IsAutomaticDataCollectActive
                                && !device.DeviceSettings.DontSaveAttendance)
                            {
                                if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AutoCollect))
                                {
                                    LoggingSystem.LogInfo("AutoCollectZkTask is calling for ZK device", device);
                                }

                                if (device.DeviceSettings.ZkDeviceSettings!= null 
                                    && device.DeviceSettings.ZkDeviceSettings.IsOldVersion)
                                {
                                    var oldRecordCount = _communicationComponent.CommunicationRecordCount(device.Id);
                                    var saveResult = _communicationComponent.DownloadAndSaveUnreadAttendancesFromSdk(device);
                                    if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AutoCollect))
                                    {
                                        LoggingSystem.LogInfo("AutoCollectZkTask save data result", new
                                        {
                                            oldRecordCount,
                                            saveResult
                                        });
                                    }

                                    if (saveResult != null
                                        && saveResult.UnknownErrorSave.IsCollectionNullOrEmpty()
                                        && saveResult.InvalidDeviceSerialNumberRecords.IsCollectionNullOrEmpty()
                                        )
                                    {
                                        var currentRecordCount = _communicationComponent.CommunicationRecordCount(device.Id);
                                        if (oldRecordCount == currentRecordCount)
                                        {
                                            _communicationComponent.CommunicationClearData(device.Id);
                                            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AutoCollect))
                                            {
                                                LoggingSystem.LogInfo("AutoCollectZkTask clear data is called");
                                            }
                                        }
                                        else
                                        {
                                            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AutoCollect))
                                            {
                                                LoggingSystem.LogInfo("AutoCollectZkTask OLD, Cannot clear data because record count", new { oldRecordCount, currentRecordCount });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AutoCollect))
                                        {
                                            LoggingSystem.LogInfo("AutoCollectZkTask OLD, Cannot clear data because of save result", ObjectHelper.SerializeAsJson(saveResult));
                                        }
                                    }
                                }
                                else
                                {
                                    _communicationComponent.DownloadAndSaveUnreadAttendancesFromSdk(device);
                                }
                            }

                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "AutoCollectZkTask Error on AutoCollectZkTask", device);
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
                if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AutoCollect))
                {
                    LoggingSystem.LogInfo("AutoCollectZkTask skipped because of lock is taken");
                }
            }
        }

    }
}
