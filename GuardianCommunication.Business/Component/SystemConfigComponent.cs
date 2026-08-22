using System;
using System.Collections.Generic;
using System.Linq;
using GuardianCommunication.Business.Cache;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.ExternalServices.KarnamaApi;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;
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

        public void ResetCache()
        {
            GuardianCommunicationInMemoryCacheWrapper.Instance.ResetSystemConfigCache();
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
                BaseUri = configCache.GuardianServiceUrl,
                Password = configCache.GuardianAppPassword,
                Username = configCache.GuardianAppUsername,
            };
            return result;
        }

        public void ConfigureApplicationEmbeddedInfo()
        {
            var karnamaComponent = new GuardianComponent(RepositoryFactory);
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


        #endregion


    }
}
