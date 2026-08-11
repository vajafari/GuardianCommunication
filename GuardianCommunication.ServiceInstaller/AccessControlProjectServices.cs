using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using GuardianCommunication.Business.Cache;
using GuardianCommunication.Business.Component;
using GuardianCommunication.Business.LiveModule;
using GuardianCommunication.Business.PrintService;
using GuardianCommunication.Business.Tasks;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Hardware.Camera.PouyaFanavaran;
using GuardianCommunication.Hardware.MetalDetectorGate.Padis;
using GuardianCommunication.Hardware.PadisController;
using GuardianCommunication.Hardware.PadisController.Model;
using GuardianCommunication.Hardware.Suprema;
using GuardianCommunication.Hardware.Suprema.SupremaConcepts.V1;
using GuardianCommunication.Hardware.Suprema.SupremaConcepts.V2;
using GuardianCommunication.Hardware.Timy;
using GuardianCommunication.Hardware.Virdi;
using GuardianCommunication.Hardware.XRayDevice;
using GuardianCommunication.Hardware.Zk;
using GuardianCommunication.Hardware.Zk.ZkConcepts;
using GuardianCommunication.Service;

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
                    AppConfigs.ValidIpAddresses,
                    AppConfigs.ConnectionConfig,
                    AppConfigs.IncludeStack,
                    AppConfigs.LogLevelGeneral1,
                    AppConfigs.LogLevelSuprema1,
                    AppConfigs.LogLevelSuprema2,
                    AppConfigs.LogLevelTimy,
                    AppConfigs.LogLevelElmoSanat,
                    AppConfigs.LogLevelPw,
                    AppConfigs.LogLevelVirdi,
                    AppConfigs.LogLevelZk,
                    AppConfigs.LogLevelKarnamaCall,
                    AppConfigs.ZkOpenDoorDelay,
                    AppConfigs.SupremaSdkAccessGroupCode,
                    AppConfigs.IsMetalDetectorGateActive,
                    AppConfigs.IsXRayActive,
                    AppConfigs.SimultaneousZkServerThreadsCount,
                }), "AppConfigs");

                LoggingSystem.LogInfo("Wait a minute to start");
                Thread.Sleep(new TimeSpan(0, 0, 1, 0));
                LoggingSystem.LogInfo("Start after wait");

                var systemConfigComponent = new SystemConfigComponent(_repositoryFactory);
                var systemConfig = systemConfigComponent.GetSystemConfig();

                LoggingSystem.LogInfo("Cache Reset Calling");
                CacheWrapper.Instance.ResetAllCaches();
                LoggingSystem.LogInfo("Cache Reset successfully");

                LoggingSystem.LogInfo("Configure Application Embedded Info Calling");
                systemConfigComponent.ConfigureApplicationEmbeddedInfo();
                LoggingSystem.LogInfo("Configure Application Embedded Info successfully");

                HardwareEventManager.Instance.ManageHardwareEvents();
                LoggingSystem.LogInfo("Hardware event manager started");

                PrintService.Instance.StartPrintService(new PrintServiceSetting
                {
                    PingTimeoutInSecond = systemConfig.SelfPrinterPingTimeoutInSecond,
                    SleepWhenQueueIsEmptyInMillisecond = systemConfig.SelfPrinterSleepWhenQueueIsEmptyInMillisecond,
                    PrinterPerQueue = systemConfig.SelfPrinterPrinterPerQueue,
                    SleepAfterPingCircleInMillisecond = systemConfig.SelfPrinterSleepAfterPingCircleInMillisecond,
                });
                LoggingSystem.LogInfo("PrintService started");

                StartTasks(systemConfig);
                LoggingSystem.LogInfo("Timers and tasks started");

                StartAllHardwareServers(systemConfig);
                LoggingSystem.LogInfo("Hardware server started");

                StartAllMetalDetectorGateServers(systemConfig);

                StartAllXRayDeviceServers(systemConfig);

                ScheduledApiCallTaskManager.Instance.StartScheduledApiCallTask();
                LoggingSystem.LogInfo("Call StartScheduledApiCallTask");

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
                new AttendanceSendToKarnamaTask(TimeSpan.FromMinutes(systemConfig.AttendanceSendToKarnamaTimerInterval)),
                new AttendanceHookTask(TimeSpan.FromMinutes(systemConfig.AttendanceHookTimerInterval)),
                new OnlineDeviceTask(TimeSpan.FromSeconds(systemConfig.OnlineDeviceTimerInterval)),
            };
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Zk))
            {
                allTasks.Add(new AutoCollectZkTask(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval)));
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Suprema))
            {
                if (ApplicationEmbeddedInfo.SupremaProducerVersions.HasFlag(SdkVersionEnumeration.SdkVersion1))
                {
                    allTasks.Add(new AutoCollectSupremaSdk1Task(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval)));
                }
                if (ApplicationEmbeddedInfo.SupremaProducerVersions.HasFlag(SdkVersionEnumeration.SdkVersion2))
                {
                    allTasks.Add(new AutoCollectSupremaSdk2Task(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval)));
                }
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Padis))
            {
                allTasks.Add(new AutoCollectPadisControllerTask(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval)));
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.ElmOSanat))
            {
                allTasks.Add(new AutoCollectElmoSanatTask(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval)));
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Timy))
            {
                allTasks.Add(new AutoCollectTimyTask(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval)));
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.ProcessingWorld))
            {
                allTasks.Add(new AutoCollectPwTask(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval)));
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Virdi))
            {
                allTasks.Add(new AutoCollectVirdiTask(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval)));
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.PouyaFanavaran))
            {
                var autoCollectTask =
                    new KarabinAutoCollectTask(TimeSpan.FromSeconds(systemConfig.KarabinCameraAutoCollectIntervalInSecond));
                allTasks.Add(autoCollectTask);
                allTasks.Add(new KarabinAccessListTask(TimeSpan.FromSeconds(systemConfig.KarabinCameraIntervalForSendAccessListInSecond)));
            }
            if (ApplicationEmbeddedInfo.ValidApplication.HasFlag(ApplicationTypeEnumeration.Self))
            {
                allTasks.Add(new SelfSendMealsTask(TimeSpan.FromMinutes(systemConfig.SelfTimerIntervalForSendMealsToDeviceInMinute)
                    , systemConfig.SelfOffsetForFutureMealsInMinute, systemConfig.SelfSendSingleFoodTitle));
            }
            if (systemConfig.IsDeleteFailedCommandsActive)
            {
                allTasks.Add(new DeleteUnsentCommandTask(TimeSpan.FromHours(systemConfig.DeleteUnsentCommandsTimerIntervalInHours)));
            }

            TaskManager.Instance.SetTimedBaseTaskList(allTasks.ToArray());
            TaskManager.Instance.SetContinuousTasksList();

        }

        private void StartAllHardwareServers(DtoSystemConfig systemConfig)
        {


            var deviceComponent = new DeviceComponent(_repositoryFactory);
            var allDevicesFromCache = deviceComponent.SearchDeviceCache(d => true);
            var deviceInfos = deviceComponent.ConvertDeviceToDeviceInfo(allDevicesFromCache);
            var cameraComponent = new CameraComponent(_repositoryFactory);
            var allCamerasFromCache = cameraComponent.SearchCameraCache(d => true);
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Zk))
            {
                try
                {
                    ZkServer.Instance.StartZkServer(new ZkAgentConfig
                    {
                        IntervalFromLastDataToReset = systemConfig.OnlineMonitoringDevicesIntervalFromLastDataToReset,
                        TimerCheckLastDataIntervalInMinutes = systemConfig.OnlineMonitoringDevicesTimerCheckLastDataIntervalInMinutes,
                        SleepAfterPingInSecond = systemConfig.OnlineMonitoringDevicesSleepAfterPingInSecond,
                        PingTimeoutInMillisecond = systemConfig.OnlineMonitoringDevicesPingTimeoutInMillisecond,
                        WaitAfterPingIsConnectedAgainInSecond = systemConfig.OnlineMonitoringDevicesWaitAfterPingIsConnectedAgainInSecond,
                        IsNetworkPingActive = systemConfig.IsNetworkPingActive,
                    }, new ZkPushConfig
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
                        , deviceInfos);
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on start ZkServer");
                }
            }

            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Padis))
            {
                try
                {
                    PadisControllerServer.Instance.StartPadisControllerServer(new PadisControllerServerConfig
                    {
                        PushServerMaxCommandLength = systemConfig.PadisControllerPushServerMaxCommandLength,
                        GrpcServerTimeoutShortInMillisecond = systemConfig.PadisControllerGrpcServerTimeoutShortInMillisecond,
                        PushServerMaxCommandCount = systemConfig.PadisControllerPushServerMaxCommandCount,
                        PushServerPushAddress = systemConfig.PadisControllerPushServerPushAddress,
                        PushServerIntervalForConsiderDeviceOnlineInSecond = systemConfig.PadisControllerPushServerIntervalForConsiderDeviceOnlineInSecond,
                        GrpcServerPort = systemConfig.PadisControllerGrpcServerPort,
                        GrpcServerTimeoutLongInMillisecond = systemConfig.PadisControllerGrpcServerTimeoutLongInMillisecond,
                        GrpcServerTimeoutVeryLongInMillisecond = systemConfig.PadisControllerGrpcServerTimeoutVeryLongInMillisecond,
                        GetCommandTimerIntervalInMillisecond = systemConfig.PadisControllerGetCommandTimerIntervalInMillisecond,
                        GrpcServerCommandCount = systemConfig.PadisControllerGrpcServerCommandCount,
                        GrpcCommandTimerIntervalInMillisecond = systemConfig.PadisControllerGrpcCommandTimerIntervalInMillisecond,
                    }
                    , _deviceCommandComponent.GetUnsentCommandsForEachDevice
                    , _deviceCommandComponent.GetUnsentCommandsCountByDeviceSerialNumberForEachDevice
                    , _attendanceComponent.ProcessServerMatchEvent
                    , deviceInfos);
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on start PadisControllerServer");
                }
            }

            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Suprema))
            {
                if (ApplicationEmbeddedInfo.SupremaProducerVersions.HasFlag(SdkVersionEnumeration.SdkVersion1))
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
                        }, _deviceCommandComponent.GetUnsentCommandsForEachDevice, deviceInfos);
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogError(exp, "Error on start SupremaSdk1Server");
                    }
                }
                if (ApplicationEmbeddedInfo.SupremaProducerVersions.HasFlag(SdkVersionEnumeration.SdkVersion2))
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
                        }, _deviceCommandComponent.GetUnsentCommandsForEachDevice, deviceInfos);
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
                        , deviceInfos
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
                    TimyServer.Instance.StartTimyServer(deviceInfos, new TimyPushConfig()
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
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.PouyaFanavaran))
            {
                try
                {
                    PouyaFanavaranServer.Instance.StartPouyaFanavaranServerServer(new KarabinAgentConfig
                    {
                        AccessFileBasePath = systemConfig.KarabinCameraAccessFileBasePath
                    }, allCamerasFromCache);
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on start PouyaFanavaranServer.Instance.StartPouyaFanavaranServerServer");
                }
            }

        }

        private void StartAllMetalDetectorGateServers(DtoSystemConfig systemConfig)
        {

            var deviceComponent = new DeviceComponent(_repositoryFactory);
            var allDevicesFromCache = deviceComponent.SearchMetalDetectorGateCache(d => true);

            if (AppConfigs.IsMetalDetectorGateActive)
            {
                PadisMetalDetectorGateServer.Instance.StartPadisMetalDetectorGateServer(
                    new PadisMetalDetectorGateConfig
                    {
                        Port = systemConfig.PadisMetalDetectorGateServerPushPort,
                        Ip = systemConfig.PadisMetalDetectorGateServerPushIp
                    }, allDevicesFromCache);
            }
        }

        private void StartAllXRayDeviceServers(DtoSystemConfig systemConfig)
        {
            var deviceComponent = new DeviceComponent(_repositoryFactory);
            var allDevicesFromCache = deviceComponent.SearchXRayDeviceCache(d => true);
            if (AppConfigs.IsXRayActive)
            {
                XRayDeviceServer.Instance.StartDeviceServer(allDevicesFromCache, new XReaDeviceServerConfig
                {
                    IntervalToRetrySendInSecond = systemConfig.XRayIntervalToRetrySendInSecond,
                    SleepAfterNoFileInMilliSecond = systemConfig.XRaySleepAfterNoFileInMilliSecond,
                    WaitBeforeAddToQueueInMilliSecond = systemConfig.XRayWaitBeforeAddToQueueInMilliSecond,
                });
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
                if (ApplicationEmbeddedInfo.SupremaProducerVersions.HasFlag(SdkVersionEnumeration.SdkVersion1))
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
                if (ApplicationEmbeddedInfo.SupremaProducerVersions.HasFlag(SdkVersionEnumeration.SdkVersion2))
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
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Padis))
            {

                try
                {
                    PadisControllerServer.Instance.StopPadisControllerServer();

                    SupremaSdk2Server.Instance.Dispose();
                    LoggingSystem.LogInfo("SupremaSdk2Server stopped");
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on Disposing SupremaSdk2Server");
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
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.PouyaFanavaran))
            {
                try
                {
                    PouyaFanavaranServer.Instance.StopServer();
                    LoggingSystem.LogInfo("PouyaFanavaranServer stopped");
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on Disposing PouyaFanavaranServer");
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

            try
            {
                ScheduledApiCallTaskManager.Instance.Dispose();
                LoggingSystem.LogInfo("ScheduledApiCallTaskManager Disposed");
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on Disposing ScheduledApiCallTaskManager");
            }

            if (AppConfigs.IsMetalDetectorGateActive)
            {
                try
                {
                    PadisMetalDetectorGateServer.Instance.StopServer();
                    LoggingSystem.LogInfo("PadisMetalDetectorGateServer StopServer called");
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on calling PadisMetalDetectorGateServer");
                }
            }

            if (AppConfigs.IsXRayActive)
            {
                XRayDeviceServer.Instance.StopServer();
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
                    ApplicationEmbeddedInfo.SerialNumber,
                    ApplicationEmbeddedInfo.CalendarType,
                    ApplicationEmbeddedInfo.CustomerName,
                    ApplicationEmbeddedInfo.DeviceCount,
                    ApplicationEmbeddedInfo.TotalDeviceCount,
                    ApplicationEmbeddedInfo.EmployeeCount,
                    ApplicationEmbeddedInfo.ExpireDate,
                    ApplicationEmbeddedInfo.ValidApplication,
                    ApplicationEmbeddedInfo.ActiveProducers,
                });
            }
        }

        #endregion

    }
}

