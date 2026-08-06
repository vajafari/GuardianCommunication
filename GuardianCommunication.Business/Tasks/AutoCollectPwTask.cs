using System;
using System.Linq;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;

namespace GuardianCommunication.Business.Tasks
{
    public class AutoCollectPwTask : TimedBaseTask
    {
        private readonly DeviceComponent _deviceComponent;
        private readonly CommunicationComponent _communicationComponent;
        private readonly SystemConfigComponent _systemConfigComponent;

        public AutoCollectPwTask(TimeSpan interval) : base(interval)
        {
            var repositoryFactory = new RepositoryFactory();
            _deviceComponent = new DeviceComponent(repositoryFactory);
            _communicationComponent = new CommunicationComponent(repositoryFactory);
            _systemConfigComponent = new SystemConfigComponent(repositoryFactory);
            LoggingSystem.LogInfo($"AutoCollectPwTask started with interval {(int)interval.TotalMinutes}");
        }

        private readonly object _lockCollect = new object();
        public override void Process()
        {
            if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.AutoCollect))
            {
                LoggingSystem.LogInfo("AutoCollectPwTask process is calling");
            }
            if (Monitor.TryEnter(_lockCollect))
            {
                try
                {

                    var allDeviceInCache = _deviceComponent.SearchDeviceCache(row => true)
                        .Where(row => row.DeviceTypeSummary.ProducerNumber == ProducerEnumeration.ProcessingWorld).ToList();

                    foreach (var device in allDeviceInCache)
                    {
                        var sleepTime = _systemConfigComponent.GetSystemConfigCache()
                            .PwSleepTimeInAutoCollectInSeconds * 1000;
                        try
                        {
                            if (device.AutomaticDataCollect && !device.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
                            {
                                if (sleepTime > 0)
                                {
                                    Thread.Sleep(sleepTime);
                                }
                                if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.AutoCollect))
                                {
                                    LoggingSystem.LogInfo("AutoCollectPwTask is starting for device", device);
                                }
                                var deviceInfo = _deviceComponent.ConvertDeviceToDeviceInfo(device);
                                var oldRecordCount = _communicationComponent.CommunicationRecordCount(deviceInfo);
                                if (sleepTime > 0)
                                {
                                    Thread.Sleep(sleepTime);
                                }
                                var saveResult = _communicationComponent.DownloadAndSaveUnreadAttendancesFromSdk(deviceInfo);
                                if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.AutoCollect))
                                {
                                    LoggingSystem.LogInfo("AutoCollectPwTask save data result", new
                                    {
                                        oldRecordCount,
                                        saveResult
                                    });
                                }

                                if (saveResult != null
                                    && saveResult.UnknownErrorSave.IsCollectionNullOrEmpty()
                                    && saveResult.InvalidDeviceSerialNumberRecords.IsCollectionNullOrEmpty())
                                {
                                    if (saveResult.AllRecords.IsCollectionNotNullOrEmpty())
                                    {
                                        if (sleepTime > 0)
                                        {
                                            Thread.Sleep(sleepTime);
                                        }
                                        var clearDataResult = _communicationComponent.CommunicationPwClearDataWithRecordCount(deviceInfo, oldRecordCount);
                                        if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.AutoCollect))
                                        {
                                            LoggingSystem.LogInfo("AutoCollectPwTask clear data result", clearDataResult);
                                        }
                                    }
                                    else
                                    {
                                        if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.AutoCollect))
                                        {
                                            LoggingSystem.LogInfo("AutoCollectPwTask No data retrieved");
                                        }
                                    }

                                }
                                else
                                {
                                    if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.AutoCollect))
                                    {
                                        LoggingSystem.LogInfo("AutoCollectPwTask Cannot clear data because of save result", saveResult);
                                    }
                                }
                            }

                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "AutoCollectPwTask Error on AutoCollectPwTask", device);
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
                if (AppConfigs.LogLevelPw.HasFlag(LogLevelPwEnumeration.AutoCollect))
                {
                    LoggingSystem.LogInfo("AutoCollectPwTask skipped because of lock is taken");
                }
            }
        }
    }
}
