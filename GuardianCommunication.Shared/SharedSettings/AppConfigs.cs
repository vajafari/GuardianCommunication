using System.Collections.Generic;
using System.Linq;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using Newtonsoft.Json;

namespace GuardianCommunication.Shared.SharedSettings
{
    public static class AppConfigs
    {
        private const string ConfigKey = "JaNdRgUkXp2s5v8y/B?D(G+KbPeShVmY";
        private static PadisConnectionStringConfig _connectionString;
        public static PadisConnectionStringConfig ConnectionString
        {
            get
            {
                if (_connectionString != null) return _connectionString;
                var configStringDecrypted = Cryptography.Decrypt(ConfigurationHelper.GetApplicationSettingValue<string>("ConnectionString"), ConfigKey);
                _connectionString = JsonConvert.DeserializeObject<PadisConnectionStringConfig>(configStringDecrypted);
                return _connectionString;
            }
        }

        private static int? _commandTimeout;
        public static int SqlCommandTimeout
        {
            get
            {
                if (_commandTimeout.HasValue) return _commandTimeout.Value;
                _commandTimeout = ConfigurationHelper.GetApplicationSettingValue<int>("SqlCommandTimeout");
                return _commandTimeout.Value;
            }
        }

        private static int? _commandTimeoutLong;
        public static int SqlCommandTimeoutLong
        {
            get
            {
                if (_commandTimeoutLong.HasValue) return _commandTimeoutLong.Value;
                _commandTimeoutLong = ConfigurationHelper.GetApplicationSettingValue<int>("SqlCommandTimeoutLong");
                return _commandTimeoutLong.Value;
            }
        }

        private static ConnectionConfiguration _connectionConfig;
        public static ConnectionConfiguration ConnectionConfig
        {
            get
            {
                if (_connectionConfig != null) return _connectionConfig;
#if DEBUG
                _connectionConfig = new ConnectionConfiguration()
                {
                    Timeout = SqlCommandTimeout,
                    LongTimeout = SqlCommandTimeoutLong,
                    ConnectionString = ConfigurationHelper.GetApplicationSettingValue<string>("ConnectionStringDevelop"),
                    KarnamaLogConnectionString = ConfigurationHelper.GetApplicationSettingValue<string>("LogConnectionStringDevelop")

                };
#else 
                var builderPadisCommunication = new SqlConnectionStringBuilder
                {
                    ApplicationName = "PadisCommunication",
                    Password = ConnectionString.SQLPassword,
                    UserID = ConnectionString.SQLUsername,
                    InitialCatalog = ConnectionString.PadisCommunicationDataBaseName,
                    DataSource = ConnectionString.ServerName,
                    MultipleActiveResultSets = true,
                    Pooling = true,
                };

                var builderKarnamaLog = new SqlConnectionStringBuilder
                {
                    ApplicationName = "PadisCommunication",
                    Password = ConnectionString.SQLPassword,
                    UserID = ConnectionString.SQLUsername,
                    InitialCatalog = ConnectionString.KarnamaLogDataBaseName,
                    DataSource = ConnectionString.ServerName,
                    MultipleActiveResultSets = true,
                    Pooling = true,
                };

                _connectionConfig = new ConnectionConfiguration
                {
                    Timeout = SqlCommandTimeout,
                    LongTimeout = SqlCommandTimeoutLong,
                    ConnectionString = builderPadisCommunication.ConnectionString,
                    KarnamaLogConnectionString = builderKarnamaLog.ConnectionString,
                };
#endif
                return _connectionConfig;
            }

        }

        private static bool? _includeStack;
        public static bool IncludeStack
        {
            get
            {
                if (_includeStack.HasValue) return _includeStack.Value;
                _includeStack = ConfigurationHelper.GetApplicationSettingValue<bool>("IncludeStack");
                return _includeStack.Value;
            }
        }

        private static List<string> _validIpAddresses;
        public static List<string> ValidIpAddresses
        {
            get
            {
                if (_validIpAddresses != null) return _validIpAddresses;

                var validIpAddressesPure = ConfigurationHelper.GetApplicationSettingValue<string>("ValidIpAddresses");
                _validIpAddresses = validIpAddressesPure.IsCollectionNullOrEmpty()
                    ? new List<string>()
                    : validIpAddressesPure.Split(',').ToList();
                return _validIpAddresses;
            }

        }

        private static int? _zkOpenDoorDelay;
        public static int ZkOpenDoorDelay
        {
            get
            {
                if (_zkOpenDoorDelay.HasValue) return _zkOpenDoorDelay.Value;
                _zkOpenDoorDelay = ConfigurationHelper.GetApplicationSettingValue<int>("ZkOpenDoorDelay");
                return _zkOpenDoorDelay.Value;
            }
        }

        private static int? _simultaneousZkServerThreadsCount;
        public static int SimultaneousZkServerThreadsCount
        {
            get
            {
                if (_simultaneousZkServerThreadsCount.HasValue) return _simultaneousZkServerThreadsCount.Value;
                _simultaneousZkServerThreadsCount = ConfigurationHelper.GetApplicationSettingValue<int>("SimultaneousZkServerThreadsCount");
                return _simultaneousZkServerThreadsCount.Value;
            }
        }

        private static bool? _isXRayActive;
        public static bool IsXRayActive
        {
            get
            {
                if (_isXRayActive.HasValue) return _isXRayActive.Value;
                _isXRayActive = ConfigurationHelper.GetApplicationSettingValue<bool>("IsXRayActive");
                return _isXRayActive.Value;
            }
        }

        private static bool? _isMetalDetectorGateActive;
        public static bool IsMetalDetectorGateActive
        {
            get
            {
                if (_isMetalDetectorGateActive.HasValue) return _isMetalDetectorGateActive.Value;
                _isMetalDetectorGateActive = ConfigurationHelper.GetApplicationSettingValue<bool>("IsMetalDetectorGateActive");
                return _isMetalDetectorGateActive.Value;
            }
        }

        private static uint? _supremaSdkAccessGroupCode;
        public static uint SupremaSdkAccessGroupCode
        {
            get
            {
                if (_supremaSdkAccessGroupCode.HasValue) return _supremaSdkAccessGroupCode.Value;
                _supremaSdkAccessGroupCode =
                    uint.TryParse(ConfigurationHelper.GetApplicationSettingValue<string>("SupremaSdkAccessGroupCode"), out var accessGroupCode)
                        ? accessGroupCode
                        : 0;
                return _supremaSdkAccessGroupCode.Value;
            }
        }


        #region LogLevels

        private static GeneralLogLevel1Enumeration? _logLevelGeneral1;
        public static GeneralLogLevel1Enumeration LogLevelGeneral1
        {
            get
            {
                if (_logLevelGeneral1.HasValue) return _logLevelGeneral1.Value;
                _logLevelGeneral1 = (GeneralLogLevel1Enumeration)ConfigurationHelper.GetApplicationSettingValue<long>("LogLevelGeneral1");
                return _logLevelGeneral1.Value;
            }
        }

        private static KarnamaCallLogLevelEnumeration? _logLevelKarnamaCall1;
        public static KarnamaCallLogLevelEnumeration LogLevelKarnamaCall
        {
            get
            {
                if (_logLevelKarnamaCall1.HasValue) return _logLevelKarnamaCall1.Value;
                _logLevelKarnamaCall1 = (KarnamaCallLogLevelEnumeration)ConfigurationHelper.GetApplicationSettingValue<long>("LogLevelKarnamaCall");
                return _logLevelKarnamaCall1.Value;
            }
        }

        private static LogLevelSuprema1Enumeration? _logLevelSuprema1;
        public static LogLevelSuprema1Enumeration LogLevelSuprema1
        {
            get
            {
                if (_logLevelSuprema1.HasValue) return _logLevelSuprema1.Value;
                _logLevelSuprema1 = (LogLevelSuprema1Enumeration)ConfigurationHelper.GetApplicationSettingValue<long>("LogLevelSuprema1");
                return _logLevelSuprema1.Value;
            }
        }

        private static LogLevelSuprema2Enumeration? _logLevelSuprema2;
        public static LogLevelSuprema2Enumeration LogLevelSuprema2
        {
            get
            {
                if (_logLevelSuprema2.HasValue) return _logLevelSuprema2.Value;
                _logLevelSuprema2 = (LogLevelSuprema2Enumeration)ConfigurationHelper.GetApplicationSettingValue<long>("LogLevelSuprema2");
                return _logLevelSuprema2.Value;
            }
        }

        private static LogLevelVirdiEnumeration? _logLevelVirdi;
        public static LogLevelVirdiEnumeration LogLevelVirdi
        {
            get
            {
                if (_logLevelVirdi.HasValue) return _logLevelVirdi.Value;
                _logLevelVirdi = (LogLevelVirdiEnumeration)ConfigurationHelper.GetApplicationSettingValue<long>("LogLevelVirdi");
                return _logLevelVirdi.Value;
            }
        }

        private static LogLevelTimyEnumeration? _logLevelTimy;
        public static LogLevelTimyEnumeration LogLevelTimy
        {
            get
            {
                if (_logLevelTimy.HasValue) return _logLevelTimy.Value;
                _logLevelTimy = (LogLevelTimyEnumeration)ConfigurationHelper.GetApplicationSettingValue<long>("LogLevelTimy");
                return _logLevelTimy.Value;
            }
        }

        private static LogLevelPwEnumeration? _logLevelPw;
        public static LogLevelPwEnumeration LogLevelPw
        {
            get
            {
                if (_logLevelPw.HasValue) return _logLevelPw.Value;
                _logLevelPw = (LogLevelPwEnumeration)ConfigurationHelper.GetApplicationSettingValue<long>("LogLevelPw");
                return _logLevelPw.Value;
            }
        }

        private static LogLevelElmoSanatEnumeration? _logLevelElmoSanat;
        public static LogLevelElmoSanatEnumeration LogLevelElmoSanat
        {
            get
            {
                if (_logLevelElmoSanat.HasValue) return _logLevelElmoSanat.Value;
                _logLevelElmoSanat = (LogLevelElmoSanatEnumeration)ConfigurationHelper.GetApplicationSettingValue<long>("LogLevelElmoSanat");
                return _logLevelElmoSanat.Value;
            }
        }

        private static LogLevelZkEnumeration? _logLevelZk;
        public static LogLevelZkEnumeration LogLevelZk
        {
            get
            {
                if (_logLevelZk.HasValue) return _logLevelZk.Value;
                _logLevelZk = (LogLevelZkEnumeration)ConfigurationHelper.GetApplicationSettingValue<long>("LogLevelZk");
                return _logLevelZk.Value;
            }
        }

        private static LogLevelPadisControllerEnumeration? _logLevelPadisController;
        public static LogLevelPadisControllerEnumeration LogLevelPadisController
        {
            get
            {
                if (_logLevelPadisController.HasValue) return _logLevelPadisController.Value;
                _logLevelPadisController = (LogLevelPadisControllerEnumeration)ConfigurationHelper.GetApplicationSettingValue<long>("LogLevelPadisController");
                return _logLevelPadisController.Value;
            }
        }

        private static LogLevelCameraEnumeration? _logLevelCamera;
        public static LogLevelCameraEnumeration LogLevelCamera
        {
            get
            {
                if (_logLevelCamera.HasValue) return _logLevelCamera.Value;
                _logLevelCamera = (LogLevelCameraEnumeration)ConfigurationHelper.GetApplicationSettingValue<long>("LogLevelCamera");
                return _logLevelCamera.Value;
            }
        }

        private static LogLevelXRayEnumeration? _logLevelXRay;
        public static LogLevelXRayEnumeration LogLevelXRay
        {
            get
            {
                if (_logLevelXRay.HasValue) return _logLevelXRay.Value;
                _logLevelXRay = (LogLevelXRayEnumeration)ConfigurationHelper.GetApplicationSettingValue<long>("LogLevelXRay");
                return _logLevelXRay.Value;
            }
        }

        private static LogLevelMetalDetectorEnumeration? _logLevelMetalDetector;
        public static LogLevelMetalDetectorEnumeration LogLevelMetalDetector
        {
            get
            {
                if (_logLevelMetalDetector.HasValue) return _logLevelMetalDetector.Value;
                _logLevelMetalDetector = (LogLevelMetalDetectorEnumeration)ConfigurationHelper.GetApplicationSettingValue<long>("LogLevelMetalDetector");
                return _logLevelMetalDetector.Value;
            }
        }

        private static LogLevelPrintServiceEnumeration? _logLevelPrintService;
        public static LogLevelPrintServiceEnumeration LogLevelPrintService
        {
            get
            {
                if (_logLevelPrintService.HasValue) return _logLevelPrintService.Value;
                _logLevelPrintService = (LogLevelPrintServiceEnumeration)ConfigurationHelper.GetApplicationSettingValue<long>("LogLevelPrintService");
                return _logLevelPrintService.Value;
            }
        }


        #endregion

    }
}
