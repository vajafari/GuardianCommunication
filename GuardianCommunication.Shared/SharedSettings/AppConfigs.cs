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
        private static GuardianConnectionStringConfig _connectionString;
        public static GuardianConnectionStringConfig ConnectionString
        {
            get
            {
                if (_connectionString != null) return _connectionString;
                var configStringDecrypted = Cryptography.Decrypt(ConfigurationHelper.GetApplicationSettingValue<string>("ConnectionString"), ConfigKey);
                _connectionString = JsonConvert.DeserializeObject<GuardianConnectionStringConfig>(configStringDecrypted);
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
                var builderGuardianCommunication = new SqlConnectionStringBuilder
                {
                    ApplicationName = "GuardianCommunication",
                    Password = ConnectionString.SQLPassword,
                    UserID = ConnectionString.SQLUsername,
                    InitialCatalog = ConnectionString.GuardianCommunicationDataBaseName,
                    DataSource = ConnectionString.ServerName,
                    MultipleActiveResultSets = true,
                    Pooling = true,
                };

                var builderKarnamaLog = new SqlConnectionStringBuilder
                {
                    ApplicationName = "GuardianCommunication",
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
                    ConnectionString = builderGuardianCommunication.ConnectionString,
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

        private static GuardianCallLogLevelEnumeration? _logLevelKarnamaCall1;
        public static GuardianCallLogLevelEnumeration LogLevelKarnamaCall
        {
            get
            {
                if (_logLevelKarnamaCall1.HasValue) return _logLevelKarnamaCall1.Value;
                _logLevelKarnamaCall1 = (GuardianCallLogLevelEnumeration)ConfigurationHelper.GetApplicationSettingValue<long>("LogLevelKarnamaCall");
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

        private static LogLevelGuardianControllerEnumeration? _logLevelGuardianController;
        public static LogLevelGuardianControllerEnumeration LogLevelGuardianController
        {
            get
            {
                if (_logLevelGuardianController.HasValue) return _logLevelGuardianController.Value;
                _logLevelGuardianController = (LogLevelGuardianControllerEnumeration)ConfigurationHelper.GetApplicationSettingValue<long>("LogLevelGuardianController");
                return _logLevelGuardianController.Value;
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

        #endregion

    }
}
