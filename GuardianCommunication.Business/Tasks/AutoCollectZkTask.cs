using System;
using System.Linq;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;

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

                    var allDeviceInCache = _deviceComponent.SearchDeviceCache(row => true)
                        .Where(row => row.DeviceTypeSummary.ProducerNumber == ProducerEnumeration.Zk).ToList();

                    foreach (var device in allDeviceInCache)
                    {
                        try
                        {
                            if (device.AutomaticDataCollect && !device.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
                            {
                                if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AutoCollect))
                                {
                                    LoggingSystem.LogInfo("AutoCollectZkTask is calling for ZK device", device);
                                }

                                if (device.IsOldVersion)
                                {
                                    var deviceInfo = _deviceComponent.ConvertDeviceToDeviceInfo(device);
                                    var oldRecordCount = _communicationComponent.CommunicationRecordCount(deviceInfo);
                                    var saveResult = _communicationComponent.DownloadAndSaveUnreadAttendancesFromSdk(deviceInfo);
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
                                        var currentRecordCount = _communicationComponent.CommunicationRecordCount(deviceInfo);
                                        if (oldRecordCount == currentRecordCount)
                                        {
                                            _communicationComponent.CommunicationClearData(deviceInfo);
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
                                    var deviceInfo = _deviceComponent.ConvertDeviceToDeviceInfo(device);
                                    _communicationComponent.DownloadAndSaveUnreadAttendancesFromSdk(deviceInfo);
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
