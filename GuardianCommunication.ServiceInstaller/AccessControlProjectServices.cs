using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Business.LiveModule;
using GuardianCommunication.Business.Tasks;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Hardware.Suprema;
using GuardianCommunication.Hardware.Suprema.SupremaConcepts.V1;
using GuardianCommunication.Hardware.Suprema.SupremaConcepts.V2;
using GuardianCommunication.Hardware.Timy;
using GuardianCommunication.Hardware.Virdi;
using GuardianCommunication.Hardware.Zk;
using GuardianCommunication.Hardware.Zk.ZkConcepts;
using GuardianCommunication.Service;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.SearchDataWrapper;
using GuardianCommunication.Shared.SharedSettings;

namespace GuardianCommunication.ServiceInstaller
{
    public partial class AccessControlProjectServices : ServiceBase
    {
        private readonly RepositoryFactory _repositoryFactory;
        private readonly DeviceCommandComponent _deviceCommandComponent;
        private readonly AttendanceComponent _attendanceComponent;
        private readonly SystemConfigComponent _systemConfigComponent;

        private ServiceHost _hardwareServiceHost;

        public AccessControlProjectServices()
        {
            _repositoryFactory = new RepositoryFactory();
            LoggingSystem.Initialize(_repositoryFactory);
            _deviceCommandComponent = new DeviceCommandComponent(_repositoryFactory);
            _attendanceComponent = new AttendanceComponent(_repositoryFactory);
            _systemConfigComponent = new SystemConfigComponent(_repositoryFactory);
            InitializeComponent();
        }


        protected override void OnStart(string[] args)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) => true;

            _hardwareServiceHost = new ServiceHost(typeof(HardwareService));
            _hardwareServiceHost.Open();
            LoggingSystem.LogInfo("HardwareService Started Successfully", "SharedService Start");

            var startUpOperations = new Thread(DoStartProcess) { IsBackground = true };
            startUpOperations.Start();

        }

        protected override void OnStop()
        {
            DoStopProcess();
            LoggingSystem.LogInfo("SettingService Stop Successfully", "SharedService Stop");
            if (_hardwareServiceHost != null && _hardwareServiceHost.State == CommunicationState.Opened)
            {
                _hardwareServiceHost.Close();
            }
            LoggingSystem.LogInfo("HardwareService Stop Successfully", "SharedService Stop");


        }


        #region Private Methods

        private void DoStartProcess()
        {
            try
            {

                LoggingSystem.LogInfo(ObjectHelper.SerializeAsJsonFormatted(new
                {
                    AppConfigs.ConnectionConfig,
                    AppConfigs.IncludeStack,
                    AppConfigs.LogLevelGeneral1,
                    AppConfigs.LogLevelSuprema1,
                    AppConfigs.LogLevelSuprema2,
                    AppConfigs.LogLevelTimy,
                    AppConfigs.LogLevelVirdi,
                    AppConfigs.LogLevelZk,
                    AppConfigs.LogLevelKarnamaCall,
                    AppConfigs.SimultaneousZkServerThreadsCount,
                }), "AppConfigs");

                LoggingSystem.LogInfo("Wait a minute to start");
                Thread.Sleep(new TimeSpan(0, 0, 1, 0));
                LoggingSystem.LogInfo("Start after wait");

                var systemConfigComponent = new SystemConfigComponent(_repositoryFactory);
                var systemConfig = systemConfigComponent.GetSystemConfig();

                LoggingSystem.LogInfo("Configure Application Embedded Info Calling");
                systemConfigComponent.ConfigureApplicationEmbeddedInfo();
                LoggingSystem.LogInfo("Configure Application Embedded Info successfully");

                HardwareEventManager.Instance.ManageHardwareEvents();
                LoggingSystem.LogInfo("Hardware event manager started");

                StartTasks(systemConfig);
                LoggingSystem.LogInfo("Timers and tasks started");

                StartAllHardwareServers(systemConfig);
                LoggingSystem.LogInfo("Hardware server started");

                DoStartUpLogging(systemConfig);

                LoggingSystem.LogInfo("All startup process done!!!!");

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on startup service");
            }


        }

        private void StartTasks(DtoSystemConfig systemConfig)
        {
            var allTasks = new List<TimedBaseTask>
            {
                new AttendanceSendToGuardianTask(TimeSpan.FromMinutes(systemConfig.AttendanceSendToGuardianTimerInterval)),
                new AttendanceHookTask(TimeSpan.FromMinutes(systemConfig.AttendanceHookTimerInterval)),
                new OnlineDeviceTask(TimeSpan.FromSeconds(systemConfig.OnlineDeviceTimerInterval)),
            };
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Zk))
            {
                allTasks.Add(new AutoCollectZkTask(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval)));
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Suprema))
            {
                if (ApplicationEmbeddedInfo.SupremaProducerVersions.Contains(SdkVersionEnumeration.SdkVersion1))
                {
                    allTasks.Add(new AutoCollectSupremaSdk1Task(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval)));
                }
                if (ApplicationEmbeddedInfo.SupremaProducerVersions.Contains(SdkVersionEnumeration.SdkVersion2))
                {
                    allTasks.Add(new AutoCollectSupremaSdk2Task(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval)));
                }
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Timy))
            {
                allTasks.Add(new AutoCollectTimyTask(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval)));
            }
            
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Virdi))
            {
                allTasks.Add(new AutoCollectVirdiTask(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval)));
            }
            
            TaskManager.Instance.SetTimedBaseTaskList(allTasks.ToArray());
            TaskManager.Instance.SetContinuousTasksList();

        }

        private void StartAllHardwareServers(DtoSystemConfig systemConfig)
        {
            var deviceComponent = new DeviceComponent(_repositoryFactory);
            var allDevices = deviceComponent
                .SearchDevice(new PagingData<DeviceFilter, DeviceSortEnumeration>());
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Zk))
            {
                try
                {
                    ZkServer.Instance.StartZkServer(new ZkPushConfig
                    {
                        PushServerIp = systemConfig.ZkPushServerIp,
                        PushServerPort = systemConfig.ZkPushServerPort,
                        MaxZkCommandCount = systemConfig.MaxZkCommandCount,
                        SleepBetweenSocketsInMillisecond = systemConfig.ZkPushSleepBetweenSocketsInMillisecond,
                        SleepOnContinueInMillisecond = systemConfig.ZkPushSleepOnContinueInMillisecond,
                        ReceiveTimeoutInMillisecond = systemConfig.ZkPushReceiveTimeoutInMillisecond,
                        Stamp = systemConfig.ZkPushStamp,
                        OpStamp = systemConfig.ZkPushOpStamp,
                        PhotoStamp = systemConfig.ZkPushPhotoStamp,
                        ErrorDelay = systemConfig.ZkPushErrorDelay,
                        Delay = systemConfig.ZkPushDelay,
                        TransTimes = systemConfig.ZkPushTransTimes,
                        TransInterval = systemConfig.ZkPushTransInterval,
                        SyncTime = systemConfig.ZkPushSyncTime,
                        Realtime = systemConfig.ZkPushRealtime,
                        AttendanceLogStamp = systemConfig.ZkPushAttendanceLogStamp,
                        OperationLogStamp = systemConfig.ZkPushOperationLogStamp,
                        AttendancePhotoStamp = systemConfig.ZkPushAttendancePhotoStamp,
                        MultiBioDataSupport = systemConfig.ZkPushMultiBioDataSupport,
                        MultiBioPhotoSupport = systemConfig.ZkPushMultiBioPhotoSupport,
                        WaitForSocketData = systemConfig.ZkPushWaitForSocketData,
                        IntervalForConsiderDeviceOnlineInSecond = systemConfig.ZkIntervalForConsiderDeviceOnlineInSecond,
                        GetCommandTimerIntervalInMillisecond = systemConfig.ZkPushGetCommandTimerIntervalInMillisecond
                    }
                        , _deviceCommandComponent.GetUnsentCommandsForEachDevice
                        , _deviceCommandComponent.GetUnsentCommandsCountByDeviceSerialNumberForEachDevice
                        , allDevices);
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on start ZkServer");
                }
            }

            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Suprema))
            {
                if (ApplicationEmbeddedInfo.SupremaProducerVersions.Contains(SdkVersionEnumeration.SdkVersion1))
                {
                    try
                    {
                        var result = BSSDK.BS_InitSDK();
                        if (result != BSSDK.BS_SUCCESS)
                        {
                            LoggingSystem.LogError("Cannot initialize SupremaSK1", "");
                        }
                        SupremaSdk1Server.Instance.StartServer(new SupremaSdk1ServerConfig
                        {
                            ServerPort = systemConfig.SupremaSdk1ServerPort,
                            CommandSetting = _systemConfigComponent.GetCommandSettingFromCache
                                (ProducerEnumeration.Suprema, SdkVersionEnumeration.SdkVersion1),
                            MaxConnections = systemConfig.SupremaSdk1ServerMaxConnection,
                            StartDelayInSecond = ServiceConstants.ServerDelayStart
                        }, _deviceCommandComponent.GetUnsentCommandsForEachDevice, allDevices);
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogError(exp, "Error on start SupremaSdk1Server");
                    }
                }
                if (ApplicationEmbeddedInfo.SupremaProducerVersions.Contains(SdkVersionEnumeration.SdkVersion2))
                {
                    try
                    {
                        SupremaSdk2Server.Instance.StartServer(new SupremaSdk2ServerConfig
                        {
                            ServerPort = systemConfig.SupremaSdk2ServerPort,
                            CommandSetting = _systemConfigComponent.GetCommandSettingFromCache
                                (ProducerEnumeration.Suprema, SdkVersionEnumeration.SdkVersion2),
                            ConnectionAliveTimer = systemConfig.SupremaSdk2ServerReconnectTimerInterval,
                            StartDelayInSecond = ServiceConstants.ServerDelayStart
                        }, _deviceCommandComponent.GetUnsentCommandsForEachDevice, allDevices);
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogError(exp, "Error on start SupremaSdk2Server");
                    }
                }
            }

            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Virdi))
            {
                try
                {
                    VirdiServer.Instance.StartVirdiServer(new VirdiServerConfig
                    {
                        ServerPort = systemConfig.VirdiServerPort,
                        CommandSetting = _systemConfigComponent.GetCommandSettingFromCache
                                        (ProducerEnumeration.Virdi, SdkVersionEnumeration.SdkVersion1),
                        SyncOperationTimeout = systemConfig.VirdiSyncOperationTimeoutInMilliseconds,
                        MaxVisibleLightImageSizeHeight = systemConfig.VirdiMaxVisibleLightImageSizeHeight,
                        MaxVisibleLightImageSizeInKb = systemConfig.VirdiMaxVisibleLightImageSizeInKb,
                        MaxVisibleLightImageSizeWidth = systemConfig.VirdiMaxVisibleLightImageSizeWidth,
                        StartDelayInSecond = ServiceConstants.ServerDelayStart
                    }
                        , _deviceCommandComponent.GetUnsentCommandsForEachDevice
                        , _attendanceComponent.ProcessServerMatchEvent
                        , allDevices
                    );
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on start SupremaSdk1Server");
                }
            }

            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Timy))
            {
                try
                {
                    TimyServer.Instance.StartTimyServer(allDevices, new TimyPushConfig()
                    {
                        NormalCommandTimeoutInSecond = systemConfig.TimyNormalCommandTimeoutInSecond,
                        LongCommandTimeoutInSecond = systemConfig.TimyLongCommandTimeoutInSecond,
                        GetCommandTimerIntervalInMillisecond = systemConfig.TimyGetCommandTimerIntervalInMillisecond,
                        WaitBetweenCommandSendInMilliseconds = systemConfig.TimyWaitBetweenCommandSendInMilliseconds,
                    }, _deviceCommandComponent.GetUnsentCommandsForEachDevice);
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on start TimyServer");
                }
            }
            
        }

        private static void DoStopProcess()
        {
            LoggingSystem.LogInfo("Stop process called");
            try
            {
                TaskManager.Instance.Dispose();
                LoggingSystem.LogInfo("TaskManager disposed");
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on Disposing TaskManager");
            }

            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Zk))
            {
                try
                {
                    ZkServer.Instance.StopServer();
                    LoggingSystem.LogInfo("ZkServer stopped");
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on Disposing ZkServer");
                }
            }

            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Timy))
            {
                try
                {
                    TimyServer.Instance.StopServer();
                    LoggingSystem.LogInfo("TimyServer stopped");
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on Disposing TimyServer");
                }
            }

            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Suprema))
            {
                if (ApplicationEmbeddedInfo.SupremaProducerVersions.Contains(SdkVersionEnumeration.SdkVersion1))
                {
                    try
                    {

                        SupremaSdk1Server.Instance.Dispose();
                        LoggingSystem.LogInfo("SupremaSdk1Server stopped");
                        BSSDK.BS_UnInitSDK();
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogError(exp, "Error on Disposing SupremaSdk1Server");
                    }
                }
                if (ApplicationEmbeddedInfo.SupremaProducerVersions.Contains(SdkVersionEnumeration.SdkVersion2))
                {
                    try
                    {
                        SupremaSdk2Server.Instance.Dispose();
                        LoggingSystem.LogInfo("SupremaSdk2Server stopped");
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogError(exp, "Error on Disposing SupremaSdk2Server");
                    }
                }
            }
            
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Virdi))
            {
                try
                {
                    VirdiServer.Instance.Dispose();
                    LoggingSystem.LogInfo("VirdiServer stopped");
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on Disposing VirdiServer");
                }
            }
            
            try
            {
                HardwareEventManager.Instance.Dispose();
                LoggingSystem.LogInfo("HardwareEventManager Disposed");
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on Disposing HardwareEventManager");
            }

        }




        private static void DoStartUpLogging(DtoSystemConfig systemConfig)
        {
            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.StartupLog))
            {
                LoggingSystem.LogInfo("System Config Data", systemConfig);
            }

            if (AppConfigs.LogLevelGeneral1.HasFlag(GeneralLogLevel1Enumeration.StartupLog))
            {
                LoggingSystem.LogInfo("Application Embedded Info", new
                {
                    ApplicationEmbeddedInfo.ExpireDate,
                    ApplicationEmbeddedInfo.ActiveProducers,
                    ApplicationEmbeddedInfo.SupremaProducerVersions,
                    ApplicationEmbeddedInfo.Modules,
                });
            }
        }

        #endregion

    }
}

