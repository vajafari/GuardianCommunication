using System;
using System.Collections.Generic;
using System.Linq;
using GuardianCommunication.Business.Cache;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.ExternalServices.KarnamaApi;
using GuardianCommunication.Shared;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.OperationResult;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.Business.Component
{
    public class SystemConfigComponent : BaseComponent
    {
        public SystemConfigComponent(RepositoryFactory sharedRepository)
            : base(sharedRepository)
        { }

        #region SystemConfig


        public DtoSystemConfig GetSystemConfig()
        {
            var result = new DtoSystemConfig();
            var allSystemConfigs = RepositoryFactory.GetSystemConfigRepository().GetAllConfigs();
            var allProperties = result.GetType().GetProperties();
            foreach (var item in allSystemConfigs)
            {
                var property = allProperties.FirstOrDefault(row => row.Name == item.ConfigName);
                if (property != null)
                {
                    property.SetValue(result, Convert.ChangeType(item.ConfigValue, property.PropertyType));
                }
            }

            return result;
        }

        public void Update(DtoSystemConfig config)
        {
            if (config == null)
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusObjectIsNull);
            }


            var configInDatabase = GetSystemConfig();
            configInDatabase.KarnamaServiceUrl = config.KarnamaServiceUrl;
            configInDatabase.AttendanceRegisterInterval = config.AttendanceRegisterInterval;
            configInDatabase.AttendanceRegisterIntervalForParking = config.AttendanceRegisterIntervalForParking;
            configInDatabase.AttendanceRegisterIntervalForTimeAttendance = config.AttendanceRegisterIntervalForTimeAttendance;
            configInDatabase.AttendanceRegisterIntervalForAccessControl = config.AttendanceRegisterIntervalForAccessControl;
            configInDatabase.OnlineDeviceTimerInterval = config.OnlineDeviceTimerInterval;
            configInDatabase.ResetRetryCountDayBefore = config.ResetRetryCountDayBefore;
            configInDatabase.KeepCommandAfterResponse = config.KeepCommandAfterResponse;
            configInDatabase.AutomaticCollectAttendanceTimerInterval = config.AutomaticCollectAttendanceTimerInterval;
            configInDatabase.OnlineMonitoringDevicesIntervalFromLastDataToReset = config.OnlineMonitoringDevicesIntervalFromLastDataToReset;
            configInDatabase.OnlineMonitoringDevicesSleepAfterPingInSecond = config.OnlineMonitoringDevicesSleepAfterPingInSecond;
            configInDatabase.OnlineMonitoringDevicesPingTimeoutInMillisecond = config.OnlineMonitoringDevicesPingTimeoutInMillisecond;
            configInDatabase.OnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond = config.OnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond;
            configInDatabase.AttendanceSendToKarnamaTimerInterval = config.AttendanceSendToKarnamaTimerInterval;
            configInDatabase.AttendanceHookTimerInterval = config.AttendanceHookTimerInterval;
            configInDatabase.AttendanceSendToKarnamaTimerRecordCount = config.AttendanceSendToKarnamaTimerRecordCount;
            configInDatabase.AttendanceHookTimerRecordCount = config.AttendanceHookTimerRecordCount;

            configInDatabase.SupremaSdk1ServerMaxConnection = config.SupremaSdk1ServerMaxConnection;
            configInDatabase.SupremaSdk1ServerPort = config.SupremaSdk1ServerPort;
            configInDatabase.SupremaSdk1DeadlineInMinutes = config.SupremaSdk1DeadlineInMinutes;
            configInDatabase.MaxRetryForSupremaSdk1UserCommands = config.MaxRetryForSupremaSdk1UserCommands;
            configInDatabase.MaxRetryForSupremaSdk1OtherCommands = config.MaxRetryForSupremaSdk1OtherCommands;

            configInDatabase.ZkPushServerIp = config.ZkPushServerIp;
            configInDatabase.ZkPushServerPort = config.ZkPushServerPort;
            configInDatabase.MaxRetryForZkOtherCommand = config.MaxRetryForZkOtherCommand;
            configInDatabase.MaxRetryForZkUserCommands = config.MaxRetryForZkUserCommands;
            configInDatabase.MaxZkCommandCount = config.MaxZkCommandCount;
            configInDatabase.ZkDeadlineInMinutes = config.ZkDeadlineInMinutes;
            //configInDatabase.ZkPushSleepBetweenSocketsInMillisecond = config.ZkPushSleepBetweenSocketsInMillisecond;
            //configInDatabase.ZkPushSleepOnContinueInMillisecond = config.ZkPushSleepOnContinueInMillisecond;
            //configInDatabase.ZkPushReceiveTimeoutInMillisecond = config.ZkPushReceiveTimeoutInMillisecond;
            //configInDatabase.ZkPushStamp = config.ZkPushStamp;
            //configInDatabase.ZkPushOpStamp = config.ZkPushOpStamp;
            //configInDatabase.ZkPushPhotoStamp = config.ZkPushPhotoStamp;
            //configInDatabase.ZkPushErrorDelay = config.ZkPushErrorDelay;
            //configInDatabase.ZkPushDelay = config.ZkPushDelay;
            //configInDatabase.ZkPushTransTimes = config.ZkPushTransTimes;
            //configInDatabase.ZkPushTransInterval = config.ZkPushTransInterval;
            //configInDatabase.ZkPushSyncTime = config.ZkPushSyncTime;
            //configInDatabase.ZkPushRealtime = config.ZkPushRealtime;
            //configInDatabase.ZkPushAttendanceLogStamp = config.ZkPushAttendanceLogStamp;
            //configInDatabase.ZkPushOperationLogStamp = config.ZkPushOperationLogStamp;
            //configInDatabase.ZkPushAttendancePhotoStamp = config.ZkPushAttendancePhotoStamp;
            //configInDatabase.ZkPushMultiBioDataSupport = config.ZkPushMultiBioDataSupport;
            //configInDatabase.ZkPushMultiBioPhotoSupport = config.ZkPushMultiBioPhotoSupport;

            configInDatabase.SupremaSdk2ServerPort = config.SupremaSdk2ServerPort;
            configInDatabase.SupremaSdk2ServerReconnectTimerInterval = config.SupremaSdk2ServerReconnectTimerInterval;
            configInDatabase.SupremaSdk2DeadlineInMinutes = config.SupremaSdk2DeadlineInMinutes;
            configInDatabase.MaxRetryForSupremaSdk2UserCommands = config.MaxRetryForSupremaSdk2UserCommands;
            configInDatabase.MaxRetryForSupremaSdk2OtherCommands = config.MaxRetryForSupremaSdk2OtherCommands;

            configInDatabase.PwDeadlineInMinutes = config.PwDeadlineInMinutes;
            configInDatabase.MaxRetryForPwUserCommands = config.MaxRetryForPwUserCommands;
            configInDatabase.MaxRetryForPwOtherCommands = config.MaxRetryForPwOtherCommands;


            configInDatabase.VirdiDeadlineInMinutes = config.VirdiDeadlineInMinutes;
            configInDatabase.MaxRetryForVirdiUserCommands = config.MaxRetryForVirdiUserCommands;
            configInDatabase.MaxRetryForVirdiOtherCommands = config.MaxRetryForVirdiOtherCommands;
            configInDatabase.VirdiServerPort = config.VirdiServerPort;

            //configInDatabase.PadisMetalDetectorGateServerPushPort = config.PadisMetalDetectorGateServerPushPort;
            //configInDatabase.PadisMetalDetectorGateServerPushIp = config.PadisMetalDetectorGateServerPushIp;



            #region Logical Validation

            if (configInDatabase.KarnamaServiceUrl.IsNullOrEmpty())
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigKarnamaServiceUrlIsNotValid);
            }
            if (!configInDatabase.AutomaticCollectAttendanceTimerInterval.IsInRange(
                ServiceConstants.HardwareServiceMinValueForAutomaticCollectAttendanceTimerInterval, ServiceConstants.HardwareServiceMaxValueForAutomaticCollectAttendanceTimerInterval))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigAutomaticCollectAttendanceTimerIntervalIsNotValid);
            }
            if (!configInDatabase.OnlineMonitoringDevicesIntervalFromLastDataToReset.IsInRange(
                ServiceConstants.HardwareServiceMinValueForOnlineMonitoringDevicesIntervalFromLastDataToReset, ServiceConstants.HardwareServiceMaxValueForOnlineMonitoringDevicesIntervalFromLastDataToReset))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigOnlineMonitoringDevicesIntervalFromLastDataToResetIsNotValid);
            }
            if (!configInDatabase.OnlineMonitoringDevicesTimerCheckLastDataIntervalInMinutes.IsInRange(
                    ServiceConstants.HardwareServiceMinValueForOnlineMonitoringDevicesTimerCheckLastDataIntervalInMinutes, ServiceConstants.HardwareServiceMaxValueForOnlineMonitoringDevicesTimerCheckLastDataIntervalInMinutes))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigOnlineMonitoringDevicesTimerCheckLastDataIntervalInMinutes);
            }
            if (!configInDatabase.OnlineMonitoringDevicesSleepAfterPingInSecond.IsInRange(
                    ServiceConstants.HardwareServiceMinValueForOnlineMonitoringDevicesSleepAfterPingInSecond, ServiceConstants.HardwareServiceMaxValueForOnlineMonitoringDevicesSleepAfterPingInSecond))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigOnlineMonitoringDevicesSleepAfterPingInSecondIsNotValid);
            }
            if (!configInDatabase.OnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond.IsInRange(
                    ServiceConstants.HardwareServiceMinValueForOnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond, ServiceConstants.HardwareServiceMaxValueForOnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigOnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecondIsNotValid);
            }
            if (!configInDatabase.OnlineMonitoringDevicesPingTimeoutInMillisecond.IsInRange(
                    ServiceConstants.HardwareServiceMinValueForOnlineMonitoringDevicesPingTimeoutInMillisecond, ServiceConstants.HardwareServiceMaxValueForOnlineMonitoringDevicesPingTimeoutInMillisecond))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigOnlineMonitoringDevicesPingTimeoutInMillisecondIsNotValid);
            }
            if (!configInDatabase.AttendanceSendToKarnamaTimerInterval.IsInRange(
                    ServiceConstants.HardwareServiceMinValueForAttendanceSendToKarnamaTimerInterval, ServiceConstants.HardwareServiceMaxValueForAttendanceSendToKarnamaTimerInterval))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigAttendanceHookTimerIntervalIsNotValid);
            }
            if (!configInDatabase.AttendanceHookTimerInterval.IsInRange(
                ServiceConstants.HardwareServiceMinValueForAttendanceHookTimerInterval, ServiceConstants.HardwareServiceMaxValueForAttendanceHookTimerInterval))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigAttendanceHookTimerIntervalIsNotValid);
            }
            if (!configInDatabase.AttendanceSendToKarnamaTimerRecordCount.IsInRange(
                    ServiceConstants.HardwareServiceMinValueForAttendanceSendToKarnamaTimerRecordCount, ServiceConstants.HardwareServiceMaxValueForAttendanceSendToKarnamaTimerRecordCount))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigAttendanceHookTimerRecordCountIsNotValid);
            }
            if (!configInDatabase.AttendanceHookTimerRecordCount.IsInRange(
                ServiceConstants.HardwareServiceMinValueForAttendanceHookTimerRecordCount, ServiceConstants.HardwareServiceMaxValueForAttendanceHookTimerRecordCount))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigAttendanceHookTimerRecordCountIsNotValid);
            }
            if (!configInDatabase.ZkPushServerPort.IsInRange(
                ServiceConstants.GeneralMinValueForPort, ServiceConstants.GeneralMaxValueForPort))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigZkPushServerPortIsNotValid);
            }
            if (configInDatabase.ZkPushServerIp.IsNullOrEmpty())
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigZkPushServerIpIsNotValid);
            }
            if (!configInDatabase.SupremaSdk1ServerMaxConnection.IsInRange(
                ServiceConstants.HardwareServiceMinValueForSupremaSdk1ServerMaxConnection, ServiceConstants.HardwareServiceMaxValueForSupremaSdk1ServerMaxConnection))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigSupremaSdk1ServerMaxConnectionIsNotValid);
            }
            if (!configInDatabase.SupremaSdk1ServerPort.IsInRange(
                ServiceConstants.GeneralMinValueForPort, ServiceConstants.GeneralMaxValueForPort))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigSupremaSdk1ServerPortIsNotValid);
            }
            if (!configInDatabase.SupremaSdk2ServerPort.IsInRange(
                ServiceConstants.GeneralMinValueForPort, ServiceConstants.GeneralMaxValueForPort))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigSupremaSdk2ServerPortIsNotValid);
            }
            if (!configInDatabase.SupremaSdk2ServerReconnectTimerInterval.IsInRange(
                ServiceConstants.HardwareServiceMinValueForSupremaSdk2ServerReconnectTimerInterval, ServiceConstants.HardwareServiceMaxValueForSupremaSdk2ServerReconnectTimerInterval))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigSupremaSdk2ServerReconnectTimerIntervalIsNotValid);
            }
            if (!configInDatabase.ResetRetryCountDayBefore.IsInRange(
                ServiceConstants.HardwareServiceMinValueForResetRetryCountDayBefore, ServiceConstants.HardwareServiceMaxValueForResetRetryCountDayBefore))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigResetRetryCountDayBeforeIsNotValid);
            }
            if (!configInDatabase.OnlineDeviceTimerInterval.IsInRange(
                ServiceConstants.HardwareServiceMinValueForOnlineDeviceTimerInterval, ServiceConstants.HardwareServiceMaxValueForOnlineDeviceTimerInterval))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigOnlineDeviceTimerIntervalIsNotValid);
            }
            if (!configInDatabase.AttendanceRegisterInterval.IsInRange(
                ServiceConstants.HardwareServiceMinValueForAttendanceRegisterInterval, ServiceConstants.HardwareServiceMaxValueForAttendanceRegisterInterval))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigAttendanceRegisterIntervalIsNotValid);
            }
            if (!configInDatabase.AttendanceRegisterIntervalForParking.IsInRange(
                    ServiceConstants.HardwareServiceMinValueForAttendanceRegisterIntervalForParking, ServiceConstants.HardwareServiceMaxValueForAttendanceRegisterIntervalForParking))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigAttendanceRegisterIntervalForParkingIsNotValid);
            }
            if (!configInDatabase.AttendanceRegisterIntervalForTimeAttendance.IsInRange(
                    ServiceConstants.HardwareServiceMinValueForAttendanceRegisterIntervalForTimeAttendance
                    , ServiceConstants.HardwareServiceMaxValueForAttendanceRegisterIntervalForTimeAttendance))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigAttendanceRegisterIntervalForTimeAttendanceIsNotValid);
            }
            if (!configInDatabase.AttendanceRegisterIntervalForAccessControl.IsInRange(
                    ServiceConstants.HardwareServiceMinValueForAttendanceRegisterIntervalForAccessControl, ServiceConstants.HardwareServiceMaxValueForAttendanceRegisterIntervalForAccessControl))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigAttendanceRegisterIntervalForAccessControlIsNotValid);
            }
            if (!configInDatabase.MaxZkCommandCount.IsInRange(
                ServiceConstants.HardwareServiceMinValueForMaxZkCommandCount, ServiceConstants.HardwareServiceMaxValueForMaxZkCommandCount))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigMaxZkCommandCountIsNotValid);
            }
            if (!configInDatabase.MaxRetryForZkOtherCommand.IsInRange(
                ServiceConstants.HardwareServiceMinValueForMaxRetryForZkOtherCommand, ServiceConstants.HardwareServiceMaxValueForMaxRetryForZkOtherCommand))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigMaxRetryForZkOtherCommandIsNotValid);
            }
            if (!configInDatabase.MaxRetryForZkUserCommands.IsInRange(
                ServiceConstants.HardwareServiceMinValueForMaxRetryForZkUserCommands, ServiceConstants.HardwareServiceMaxValueForMaxRetryForZkUserCommands))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigMaxRetryForZkUserCommandsIsNotValid);
            }
            if (!configInDatabase.ZkDeadlineInMinutes.IsInRange(
                ServiceConstants.HardwareServiceMinValueForZkDeadlineInMinutes, ServiceConstants.HardwareServiceMaxValueForZkDeadlineInMinutes))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigZkDeadlineInMinutesIsNotValid);
            }
            if (!configInDatabase.PwDeadlineInMinutes.IsInRange(
                ServiceConstants.HardwareServiceMinValueForPwDeadlineInMinutes, ServiceConstants.HardwareServiceMaxValueForPwDeadlineInMinutes))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigPwDeadlineInMinutesIsNotValid);
            }
            if (!configInDatabase.MaxRetryForPwUserCommands.IsInRange(
                ServiceConstants.HardwareServiceMinValueForMaxRetryForPwUserCommands, ServiceConstants.HardwareServiceMaxValueForMaxRetryForPwUserCommands))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigMaxRetryForPwUserCommandsIsNotValid);
            }
            if (!configInDatabase.MaxRetryForPwOtherCommands.IsInRange(
                ServiceConstants.HardwareServiceMinValueForMaxRetryForPwOtherCommands, ServiceConstants.HardwareServiceMaxValueForMaxRetryForPwOtherCommands))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigMaxRetryForMaxRetryForPwOtherCommandsIsNotValid);
            }
            if (!configInDatabase.VirdiDeadlineInMinutes.IsInRange(
                ServiceConstants.HardwareServiceMinValueForVirdiDeadlineInMinutes, ServiceConstants.HardwareServiceMaxValueForVirdiDeadlineInMinutes))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigVirdiDeadlineInMinutesIsNotValid);
            }
            if (!configInDatabase.MaxRetryForVirdiUserCommands.IsInRange(
                ServiceConstants.HardwareServiceMinValueForMaxRetryForVirdiUserCommands, ServiceConstants.HardwareServiceMaxValueForMaxRetryForVirdiUserCommands))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigMaxRetryForVirdiUserCommandsIsNotValid);
            }
            if (!configInDatabase.MaxRetryForVirdiOtherCommands.IsInRange(
                ServiceConstants.HardwareServiceMinValueForMaxRetryForVirdiOtherCommands, ServiceConstants.HardwareServiceMaxValueForMaxRetryForVirdiOtherCommands))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigMaxRetryForMaxRetryForVirdiOtherCommandsIsNotValid);
            }

            if (!configInDatabase.SupremaSdk1DeadlineInMinutes.IsInRange(
                ServiceConstants.HardwareServiceMinValueForSupremaSdk1DeadlineInMinutes, ServiceConstants.HardwareServiceMaxValueForSupremaSdk1DeadlineInMinutes))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigSupremaSdk1DeadlineInMinutesIsNotValid);
            }
            if (!configInDatabase.MaxRetryForSupremaSdk1UserCommands.IsInRange(
                ServiceConstants.HardwareServiceMinValueForMaxRetryForSupremaSdk1UserCommands, ServiceConstants.HardwareServiceMaxValueForMaxRetryForSupremaSdk1UserCommands))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigMaxRetryForSupremaSdk1UserCommandsIsNotValid);
            }
            if (!configInDatabase.MaxRetryForSupremaSdk1OtherCommands.IsInRange(
                ServiceConstants.HardwareServiceMinValueForMaxRetryForSupremaSdk1OtherCommands, ServiceConstants.HardwareServiceMaxValueForMaxRetryForSupremaSdk1OtherCommands))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigMaxRetryForMaxRetryForSupremaSdk1OtherCommandsIsNotValid);
            }

            if (!configInDatabase.SupremaSdk2DeadlineInMinutes.IsInRange(
                ServiceConstants.HardwareServiceMinValueForSupremaSdk2DeadlineInMinutes, ServiceConstants.HardwareServiceMaxValueForSupremaSdk2DeadlineInMinutes))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigSupremaSdk2DeadlineInMinutesIsNotValid);
            }
            if (!configInDatabase.MaxRetryForSupremaSdk2UserCommands.IsInRange(
                ServiceConstants.HardwareServiceMinValueForMaxRetryForSupremaSdk2UserCommands, ServiceConstants.HardwareServiceMaxValueForMaxRetryForSupremaSdk2UserCommands))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigMaxRetryForSupremaSdk2UserCommandsIsNotValid);
            }
            if (!configInDatabase.MaxRetryForSupremaSdk2OtherCommands.IsInRange(
                ServiceConstants.HardwareServiceMinValueForMaxRetryForSupremaSdk2OtherCommands, ServiceConstants.HardwareServiceMaxValueForMaxRetryForSupremaSdk2OtherCommands))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigMaxRetryForMaxRetryForSupremaSdk2OtherCommandsIsNotValid);
            }
            if (!configInDatabase.PadisMetalDetectorGateServerPushPort.IsInRange(
                    ServiceConstants.GeneralMinValueForPort, ServiceConstants.GeneralMaxValueForPort))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration.SystemConfigStatusHardwareConfigPadisMetalDetectorGateServerPushPortIsNotValid);
            }
            #endregion



            UpdateGeneral(configInDatabase);

        }

        public DtoSystemConfigDeviceCommandSetting GetCommandSettingFromCache(ProducerEnumeration producer, SdkVersionEnumeration sdkVersion)
        {
            var configCache = GetSystemConfigCache();
            var result = new DtoSystemConfigDeviceCommandSetting
            {
                SleepBetweenSendsIfCommandNotExistsInMilliSeconds = configCache.SleepBetweenSendsIfCommandNotExistsInMilliSeconds,
            };
            switch (producer)
            {
                case ProducerEnumeration.Zk:
                    result.MaxRetryForOtherCommand = configCache.MaxRetryForZkOtherCommand;
                    result.MaxRetryForUserCommand = configCache.MaxRetryForZkUserCommands;
                    result.WaitBetweenCommandSendInMilliseconds = configCache.ZkWaitInCommandLoopInMilliseconds;
                    break;
                case ProducerEnumeration.Timy:
                    result.MaxRetryForOtherCommand = configCache.TimyMaxRetryForOtherCommand;
                    result.MaxRetryForUserCommand = configCache.TimyMaxRetryForUserCommand;
                    result.WaitBetweenCommandSendInMilliseconds = configCache.TimyWaitBetweenCommandSendInMilliseconds;
                    break;
                case ProducerEnumeration.Suprema:
                    switch (sdkVersion)
                    {
                        case SdkVersionEnumeration.SdkVersion1:
                            result.MaxRetryForOtherCommand = configCache.MaxRetryForSupremaSdk1OtherCommands;
                            result.MaxRetryForUserCommand = configCache.MaxRetryForSupremaSdk1UserCommands;
                            result.WaitBetweenCommandSendInMilliseconds = configCache.SupremaSdk1WaitInCommandLoopInMilliseconds;
                            break;
                        case SdkVersionEnumeration.SdkVersion2:
                            result.MaxRetryForOtherCommand = configCache.MaxRetryForSupremaSdk2OtherCommands;
                            result.MaxRetryForUserCommand = configCache.MaxRetryForSupremaSdk2UserCommands;
                            result.WaitBetweenCommandSendInMilliseconds = configCache.SupremaSdk2WaitInCommandLoopInMilliseconds;
                            break;
                    }
                    break;
                case ProducerEnumeration.Virdi:
                    result.MaxRetryForOtherCommand = configCache.MaxRetryForVirdiOtherCommands;
                    result.MaxRetryForUserCommand = configCache.MaxRetryForVirdiUserCommands;
                    result.WaitBetweenCommandSendInMilliseconds = configCache.VirdiWaitInCommandLoopInMilliseconds;
                    break;
            }
            return result;
        }

        public GuardianApiConfig GetGuardianApiConfig()
        {
            var configCache = GetSystemConfigCache();
            var result = new GuardianApiConfig
            {
                BaseUri = configCache.KarnamaServiceUrl,
                Password = configCache.KarnamaAppPassword,
                Username = configCache.KarnamaAppUsername,
            };
            return result;
        }

        public void ConfigureApplicationEmbeddedInfo()
        {
            var karnamaComponent = new KarnamaComponent(RepositoryFactory);
            var encodedConfig = karnamaComponent.GetSoftwareEncodedConfig();
            ApplicationEmbeddedInfo.Modules = encodedConfig.Modules;
            ApplicationEmbeddedInfo.ExpireDate = encodedConfig.ExpireDate;
            ApplicationEmbeddedInfo.ActiveProducers = encodedConfig.ActiveProducers;
            ApplicationEmbeddedInfo.SupremaProducerVersions = encodedConfig.SupremaProducerVersions;

            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.StartupLog))
            {
                LoggingSystem.LogInfo("ApplicationEmbeddedInfo", ObjectHelper.SerializeAsJsonFormatted(new
                {
                    ApplicationEmbeddedInfo.Modules,
                    ApplicationEmbeddedInfo.ActiveProducers,
                    ApplicationEmbeddedInfo.ExpireDate,
                    ApplicationEmbeddedInfo.SupremaProducerVersions,
                }));
            }

        }


        #region Internal Methods

        internal List<DtoAttendanceRegisterIntervalSetting> GetAttendanceRegisterIntervalSetting(ModuleEnumeration moduleType)
        {
            var result = new List<DtoAttendanceRegisterIntervalSetting>();
            var configCache = GetSystemConfigCache();
            if (configCache.AttendanceRegisterInterval > 0)
            {
                result.Add(new DtoAttendanceRegisterIntervalSetting
                {
                    ApplicationType = AttendanceRegisterIntervalTypeEnumeration.General,
                    Interval = configCache.AttendanceRegisterInterval,
                });
            }
            if (moduleType.HasFlag(ModuleEnumeration.Parking) && configCache.AttendanceRegisterIntervalForParking > 0)
            {
                result.Add(new DtoAttendanceRegisterIntervalSetting
                {
                    ApplicationType = AttendanceRegisterIntervalTypeEnumeration.Parking,
                    Interval = configCache.AttendanceRegisterIntervalForParking,
                });
            }
            return result;
        }

        internal DtoSystemConfig GetSystemConfigCache()
        {
            return GuardianCommunicationInMemoryCacheWrapper.Instance.GetSystemConfigCache();
        }

        #endregion


        #region Private Methods

        private void UpdateGeneral(DtoSystemConfig configInDatabase)
        {

            var entities = new List<DtoSystemConfigPure>();
            var allProperties = configInDatabase.GetType().GetProperties();
            foreach (var item in allProperties)
            {
                entities.Add(new DtoSystemConfigPure
                {
                    ConfigName = item.Name,
                    ConfigValue = item.GetValue(configInDatabase) == null ? null : item.GetValue(configInDatabase).ToString()
                });
            }

            RepositoryFactory.GetSystemConfigRepository().Update(entities);

            try
            {
                ResetCacheAfterUpdate();
            }
            catch (Exception exp)
            {
                throw new OperationCannotBeDoneException(CommunicationSharedResource.CacheResetAdditionalInformation.FormatInvariantCulture
                        ("SystemConfig", exp.GetFullExceptionMessage(), ""),
                    OperationResultEnumeration.CommunicationStatusFaultCausedCacheDataReset);
            }


        }

        private static void ResetCacheAfterUpdate()
        {
            GuardianCommunicationInMemoryCacheWrapper.Instance.ResetSystemConfigCache();
        }

        #endregion



        #endregion


    }
}
