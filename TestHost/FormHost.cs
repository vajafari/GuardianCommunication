using Communication.Business.Cache;
using Communication.Business.Component;
using Communication.Business.LiveModule;
using Communication.Business.PrintService;
using Communication.Business.Tasks;
using Communication.Data.Logger;
using Communication.Data.Repository;
using Communication.Hardware.Camera.PouyaFanavaran;
using Communication.Hardware.MetalDetectorGate.Padis;
using Communication.Hardware.PadisController;
using Communication.Hardware.PadisController.Model;
using Communication.Hardware.Shared;
using Communication.Hardware.Suprema;
using Communication.Hardware.Suprema.SupremaConcepts.V1;
using Communication.Hardware.Suprema.SupremaConcepts.V2;
using Communication.Hardware.Timy;
using Communication.Hardware.Virdi;
using Communication.Hardware.XRayDevice;
using Communication.Hardware.Zk;
using Communication.Service;
using Communication.Shared.Definition;
using Communication.Shared.Dto;
using Communication.Shared.ExtensionsAndUtilities;
using Communication.Shared.Filter;
using Communication.Shared.HardwareDefinition;
using Communication.Shared.SharedSettings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading;
using System.Windows.Forms;

//using Communication.Service;
//using Communication.ServiceShared.SharedSettings;
//using Communication.Shared.Definition;

namespace TestHost
{
    public partial class FormHost : Form
    {
        private readonly Random _randomNumberGenerator = new Random();
        private readonly RepositoryFactory _repositoryFactory;
        private readonly DeviceCommandComponent _deviceCommandComponent;
        private readonly DeviceComponent _deviceComponent;
        private readonly OtherHardwareCommandComponent _otherHardwareCommandComponent;
        private readonly AttendanceComponent _attendanceComponent;
        private readonly SystemConfigComponent _systemConfigComponent;
        private ServiceHost _hardwareHost;
        private AutoCollectPwTask _autoCollectPwTask;
        private AutoCollectZkTask _autoCollectZkTask;
        private AutoCollectTimyTask _autoCollectTimyTask;
        private AutoCollectElmoSanatTask _autoCollectElmoSanatTask;
        private AutoCollectSupremaSdk1Task _autoCollectSupremaSdk1Task;
        private AutoCollectSupremaSdk2Task _autoCollectSupremaSdk2Task;
        private AutoCollectPadisControllerTask _autoCollectPadisControllerTask;
        private AutoCollectVirdiTask _autoCollectVirdiTask;
        private AttendanceSendToKarnamaTask _attendanceSendToKarnamaTask;
        private AttendanceHookTask _attendanceHookTask;
        private OnlineDeviceTask _onlineDeviceTask;
        private SelfSendMealsTask _selfSendMeals;
        private DeleteUnsentCommandTask _deleteFailedCommandTask;

        private KarabinAutoCollectTask _karabinAutoCollectTask;
        private KarabinAccessListTask _karabinAccessListTask;




        public FormHost()
        {
            _repositoryFactory = new RepositoryFactory();
            LoggingSystem.Initialize(_repositoryFactory);
            _otherHardwareCommandComponent = new OtherHardwareCommandComponent(_repositoryFactory);
            _deviceCommandComponent = new DeviceCommandComponent(_repositoryFactory);
            _deviceComponent = new DeviceComponent(_repositoryFactory);
            _attendanceComponent = new AttendanceComponent(_repositoryFactory);
            _systemConfigComponent = new SystemConfigComponent(_repositoryFactory);
            InitializeComponent();
            InitializeTimyAccessTab();
        }

        private void FormTest_Load(object sender, EventArgs e)
        {

            DoStartProcess();
            btnHostAll.PerformClick();
            cmbEvent.SelectedIndex = 0;
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            // ReSharper disable LocalizableElement
            Text = $"My Application Version {version}";
            // ReSharper restore LocalizableElement
        }

        private void btnHostHardwareService_Click(object sender, EventArgs e)
        {

            if (_hardwareHost != null && _hardwareHost.State == CommunicationState.Opened)
            {
                ShowMessage("HardwareService Service Is Open");
                return;
            }

            try
            {
                _hardwareHost = new WebServiceHost(typeof(HardwareService));
                _hardwareHost.Open();
                ShowMessage("Success HardwareService");
            }
            catch (Exception exp)
            {
                ShowMessage(exp.Message);
            }

        }

        private void btnCloseHardwareService_Click(object sender, EventArgs e)
        {

            try
            {
                if (_hardwareHost != null && _hardwareHost.State == CommunicationState.Opened)
                {
                    _hardwareHost.Close();
                }
                ShowMessage("HardwareService Success");

            }
            catch (Exception exp)
            {
                ShowMessage(exp.Message);
            }
        }

        private void btnHostAll_Click(object sender, EventArgs e)
        {
            btnHostHardwareService.PerformClick();
        }

        private void btnCloseAll_Click(object sender, EventArgs e)
        {
            btnCloseHardwareService.PerformClick();
        }

        private void ShowMessage(string message)
        {
            if (chkShowMessage.Checked)
            {
                MessageBox.Show(message);
            }
        }

        private void btnRaiseEvent_Click(object sender, EventArgs e)
        {
            try
            {
                switch (cmbEvent.SelectedIndex)
                {
                    case 0:
                        _autoCollectSupremaSdk1Task.Process();
                        break;
                    case 1:
                        _autoCollectSupremaSdk2Task.Process();
                        break;
                    case 2:
                        _autoCollectVirdiTask.Process();
                        break;
                    case 3:
                        _autoCollectPwTask.Process();
                        break;
                    case 4:
                        _autoCollectZkTask.Process();
                        break;
                    case 5:
                        _autoCollectTimyTask.Process();
                        break;
                    case 6:
                        _autoCollectElmoSanatTask.Process();
                        break;
                    case 7:
                        _attendanceHookTask.Process();
                        break;
                    case 8:
                        _attendanceSendToKarnamaTask.Process();
                        break;
                    case 9:
                        _onlineDeviceTask.Process();
                        break;
                    case 10:
                        try
                        {
                            var component = new KarnamaComponent(_repositoryFactory);
                            component.SendDeviceEventLog(new DtoDeviceEventLog
                            {
                                DeviceNumber = 2,
                                EmployeeNumber = 1,
                                EventCode = 0x2200,
                                EventDateTime = DateTime.Now,
                                Id = 1,
                                IsFromDevice = true,
                                Producer = ProducerEnumeration.Suprema,
                                SdkVersion = SdkVersionEnumeration.SdkVersion2
                            });
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                        break;
                    case 11:
                        break;
                    case 12:
                        _selfSendMeals.Process();
                        break;
                    case 13:
                        break;
                    case 14:
                        _karabinAccessListTask.Process();
                        break;
                    case 15:
                        _karabinAutoCollectTask.Process();
                        break;
                    case 16:
                        _deleteFailedCommandTask.Process();
                        break;
                    case 17:
                        _autoCollectPadisControllerTask.Process();
                        break;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.GetFullExceptionMessage());
            }
        }


        #region Private Methods

        private void DoStartProcess()
        {
            try
            {
                Console.WriteLine(AppConfigs.SimultaneousZkServerThreadsCount);
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

                PrintService.Instance.StartPrintService(new PrintServiceSetting()
                {
                    PingTimeoutInSecond = systemConfig.SelfPrinterPingTimeoutInSecond,
                    SleepWhenQueueIsEmptyInMillisecond = systemConfig.SelfPrinterSleepWhenQueueIsEmptyInMillisecond,
                    PrinterPerQueue = systemConfig.SelfPrinterPrinterPerQueue,
                    SleepAfterPingCircleInMillisecond = systemConfig.SelfPrinterSleepAfterPingCircleInMillisecond,
                });

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
                MessageBox.Show(exp.GetFullExceptionMessage());
            }


        }

        private void StartTasks(DtoSystemConfig systemConfig)
        {
            _attendanceSendToKarnamaTask =
                new AttendanceSendToKarnamaTask(
                    TimeSpan.FromMinutes(systemConfig.AttendanceSendToKarnamaTimerInterval));
            _attendanceHookTask = new AttendanceHookTask(TimeSpan.FromMinutes(systemConfig.AttendanceHookTimerInterval));
            _onlineDeviceTask = new OnlineDeviceTask(TimeSpan.FromSeconds(systemConfig.OnlineDeviceTimerInterval));

            var allTasks = new List<TimedBaseTask>
            {
                _attendanceSendToKarnamaTask,
                _attendanceHookTask,
                _onlineDeviceTask,
            };

            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Zk))
            {
                _autoCollectZkTask =
                    new AutoCollectZkTask(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval));
                allTasks.Add(_autoCollectZkTask);
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Suprema))
            {
                if (ApplicationEmbeddedInfo.SupremaProducerVersions.HasFlag(SdkVersionEnumeration.SdkVersion1))
                {
                    _autoCollectSupremaSdk1Task =
                        new AutoCollectSupremaSdk1Task(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval));
                    allTasks.Add(_autoCollectSupremaSdk1Task);
                }
                if (ApplicationEmbeddedInfo.SupremaProducerVersions.HasFlag(SdkVersionEnumeration.SdkVersion2))
                {
                    _autoCollectSupremaSdk2Task =
                        new AutoCollectSupremaSdk2Task(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval));
                    allTasks.Add(_autoCollectSupremaSdk2Task);
                }
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Padis))
            {
                _autoCollectPadisControllerTask =
                    new AutoCollectPadisControllerTask(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval));
                allTasks.Add(_autoCollectPadisControllerTask);
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.ElmOSanat))
            {
                _autoCollectElmoSanatTask =
                    new AutoCollectElmoSanatTask(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval));
                allTasks.Add(_autoCollectElmoSanatTask);
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Timy))
            {
                _autoCollectTimyTask =
                    new AutoCollectTimyTask(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval));
                allTasks.Add(_autoCollectTimyTask);
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.ProcessingWorld))
            {
                _autoCollectPwTask =
                    new AutoCollectPwTask(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval));
                allTasks.Add(_autoCollectPwTask);
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Virdi))
            {
                _autoCollectVirdiTask =
                    new AutoCollectVirdiTask(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval));
                allTasks.Add(_autoCollectVirdiTask);
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.PouyaFanavaran))
            {
                _karabinAutoCollectTask =
                    new KarabinAutoCollectTask(
                        TimeSpan.FromSeconds(systemConfig.KarabinCameraAutoCollectIntervalInSecond));
                allTasks.Add(_karabinAutoCollectTask);
                _karabinAccessListTask = new KarabinAccessListTask(
                        TimeSpan.FromSeconds(systemConfig.KarabinCameraIntervalForSendAccessListInSecond));
                allTasks.Add(_karabinAccessListTask);
            }
            if (ApplicationEmbeddedInfo.ValidApplication.HasFlag(ApplicationTypeEnumeration.Self))
            {
                _selfSendMeals = new SelfSendMealsTask(TimeSpan.FromMinutes(systemConfig.SelfTimerIntervalForSendMealsToDeviceInMinute),
                        systemConfig.SelfOffsetForFutureMealsInMinute, systemConfig.SelfSendSingleFoodTitle);
                allTasks.Add(_selfSendMeals);
            }
            if (systemConfig.IsDeleteFailedCommandsActive)
            {
                _deleteFailedCommandTask = new DeleteUnsentCommandTask(TimeSpan.FromHours(systemConfig.DeleteUnsentCommandsTimerIntervalInHours));
                allTasks.Add(_deleteFailedCommandTask);
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
                        GrpcServerPort = systemConfig.PadisControllerGrpcServerPort,
                        PushServerMaxCommandLength = systemConfig.PadisControllerPushServerMaxCommandLength,
                        PushServerIntervalForConsiderDeviceOnlineInSecond = systemConfig.PadisControllerPushServerIntervalForConsiderDeviceOnlineInSecond,
                        PushServerMaxCommandCount = systemConfig.PadisControllerPushServerMaxCommandCount,
                        PushServerPushAddress = systemConfig.PadisControllerPushServerPushAddress,
                        GetCommandTimerIntervalInMillisecond = systemConfig.PadisControllerGetCommandTimerIntervalInMillisecond,
                        GrpcServerTimeoutShortInMillisecond = systemConfig.PadisControllerGrpcServerTimeoutShortInMillisecond,
                        GrpcServerTimeoutLongInMillisecond = systemConfig.PadisControllerGrpcServerTimeoutLongInMillisecond,
                        GrpcServerTimeoutVeryLongInMillisecond = systemConfig.PadisControllerGrpcServerTimeoutVeryLongInMillisecond,
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

            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Suprema))
            {
                if (ApplicationEmbeddedInfo.SupremaProducerVersions.HasFlag(SdkVersionEnumeration.SdkVersion1))
                {
                    try
                    {
                        var result = BSSDK.BS_InitSDK();
                        if (result != BSSDK.BS_SUCCESS)
                        {
                            LoggingSystem.LogError("Cannot initialize SupremaSK1", "Cannot initialize SupremaSK1");
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
                        StartDelayInSecond = ServiceConstants.ServerDelayStart,
                        MaxVisibleLightImageSizeHeight = systemConfig.VirdiMaxVisibleLightImageSizeHeight,
                        MaxVisibleLightImageSizeInKb = systemConfig.VirdiMaxVisibleLightImageSizeInKb,
                        MaxVisibleLightImageSizeWidth = systemConfig.VirdiMaxVisibleLightImageSizeWidth,
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
                    new PadisMetalDetectorGateConfig()
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
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on Stop TimyServer");
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

            try
            {
                ScheduledApiCallTaskManager.Instance.Dispose();
                LoggingSystem.LogInfo("ScheduledApiCallTaskManager Disposed");
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on Disposing ScheduledApiCallTaskManager");
            }

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


        private void btnTest_Click(object sender, EventArgs e)
        {
            var result1 = _deviceCommandComponent.GetUnsentCommandsCountByDeviceSerialNumberForEachDevice(new DeviceNotSentCommandsFilter()
            {
                Producer = ProducerEnumeration.Zk,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                DeviceSerialNumbers = new List<string>() { "1", "2", "3" }
            });
            var result2 = _deviceCommandComponent.GetUnsentCommandsCountByDeviceNumberForEachDevice(new DeviceNotSentCommandsFilter()
            {
                Producer = ProducerEnumeration.Zk,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                DeviceNumbers = new List<int>() { 1, 2, 3 }
            });


            var ids = new List<long>();
            for (int i = 0; i < 2500; i++)
            {
                ids.Add(i);
            }

            // _repositoryFactory.GetAttendanceHookSystemRepository().ResetHookByIds(ids);
            //_deviceCommandComponent.GetUnsentCommandsForEachDevice(new DeviceNotSentCommandsFilter
            //{
            //    Producer = ProducerEnumeration.Suprema,
            //    SdkVersion = SdkVersionEnumeration.SdkVersion1,
            //    Count = 10,
            //    DeviceSerialNumbers = _deviceComponent.SearchDeviceCache(c => true).Select(d => d.SerialNumber).ToList()
            //});

            //SdkVersion { get; set;
            //    }
            //Count { get; set; }
            //    DeviceNumbers { get; set; }
            //    DeviceSerialNumbers { get; set; }
            //});
        }


        private void btnPrint_Click(object sender, EventArgs e)
        {
            var rnd = new Random();
            var deviceNumber = rnd.Next(1, 1);
            string printerName;
            string printerIp;
            switch (deviceNumber)
            {
                default:
                    printerName = txtPrinterName.Text;
                    printerIp = txtPrinterIp.Text;
                    break;
            }
            var billInfo = new DtoSelfBillInfo
            {
                Id = _randomNumberGenerator.Next(),
                DeviceNumber = deviceNumber,
                PrinterName = printerName,
                PrinterIp = printerIp,
                EmployeeTitle = txtEmployeeTitle.Text,
                FishNumber = txtFishNumber.Text,
                IssueDate = new DateTime(2022, 12, 12, 12, 12, 12),
                ManualBill = txtManualFishString.Text,
                HeaderTitles = new List<string>(),
                FoodInfos = new List<DtoSelfBillFoodInfo>()
            };
            if (txtSelfPrintInfoHeaderTitle1.Text.IsNotNullOrEmpty())
            {
                billInfo.HeaderTitles.Add(txtSelfPrintInfoHeaderTitle1.Text);
            }
            if (txtSelfPrintInfoHeaderTitle2.Text.IsNotNullOrEmpty())
            {
                billInfo.HeaderTitles.Add(txtSelfPrintInfoHeaderTitle2.Text);
            }
            if (txtSelfPrintInfoHeaderTitle3.Text.IsNotNullOrEmpty())
            {
                billInfo.HeaderTitles.Add(txtSelfPrintInfoHeaderTitle3.Text);
            }
            if (txtSelfPrintInfoHeaderTitle4.Text.IsNotNullOrEmpty())
            {
                billInfo.HeaderTitles.Add(txtSelfPrintInfoHeaderTitle4.Text);
            }

            if (txtSelfPrintInfoFoodTitle1.Text.IsNotNullOrEmpty())
            {
                billInfo.FoodInfos.Add(new DtoSelfBillFoodInfo
                {
                    FoodTitle = txtSelfPrintInfoFoodTitle1.Text,
                    Description = txtSelfPrintInfoFoodDescription1.Text,
                    FishCount = txtSelfPrintInfoFishCount1.Text,
                    FoodType = txtSelfPrintInfoFoodType1.Text,
                    Price = txtSelfPrintInfoFoodPrice1.Text,
                    IsAllowed = chkSelfPrintInfoFoodIsAllowed1.Checked,
                    Separator = txtSelfPrintInfoFoodSeparator.Text,
                });
            }
            if (txtSelfPrintInfoFoodTitle2.Text.IsNotNullOrEmpty())
            {
                billInfo.FoodInfos.Add(new DtoSelfBillFoodInfo
                {
                    FoodTitle = txtSelfPrintInfoFoodTitle2.Text,
                    Description = txtSelfPrintInfoFoodDescription2.Text,
                    FishCount = txtSelfPrintInfoFishCount2.Text,
                    FoodType = txtSelfPrintInfoFoodType2.Text,
                    Price = txtSelfPrintInfoFoodPrice2.Text,
                    IsAllowed = chkSelfPrintInfoFoodIsAllowed2.Checked,
                    Separator = txtSelfPrintInfoFoodSeparator.Text,
                });
            }
            if (txtSelfPrintInfoFoodTitle3.Text.IsNotNullOrEmpty())
            {
                billInfo.FoodInfos.Add(new DtoSelfBillFoodInfo
                {
                    FoodTitle = txtSelfPrintInfoFoodTitle3.Text,
                    Description = txtSelfPrintInfoFoodDescription3.Text,
                    FishCount = txtSelfPrintInfoFishCount3.Text,
                    FoodType = txtSelfPrintInfoFoodType3.Text,
                    Price = txtSelfPrintInfoFoodPrice3.Text,
                    IsAllowed = chkSelfPrintInfoFoodIsAllowed3.Checked,
                    Separator = txtSelfPrintInfoFoodSeparator.Text,
                });
            }
            if (chkSelfPrintInfoDirect.Checked)
            {
                PrintService.Instance.DirectPrintSelfBill(billInfo);
            }
            else
            {
                PrintService.Instance.AddToSelfBillQueue(billInfo);
            }


        }

        private void btnShowLatestImage_Click(object sender, EventArgs e)
        {
            //var selectedCamera = bsCamera.Current as DtoCamera;
            //if (selectedCamera == null)
            //{
            //    MessageBox.Show("Please select camera");
            //    return;
            //}

            //var result = OptimizedCameraServer.Instance.TakePhotoImage(selectedCamera);
            //pbCameraLastImage.Image = ByteToImage(result.ImageBytes);
        }

        public static Bitmap ByteToImage(byte[] blob)
        {
            var mStream = new MemoryStream();
            var pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            var bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }

        private void btnDoStopProcess_Click(object sender, EventArgs e)
        {
            DoStopProcess();
        }

        private void btnSendFoodTitles_Click(object sender, EventArgs e)
        {
            if (txtSendFoodTitles.Text.IsNotNullOrEmpty()
                && txtSendFoodTitlesDeviceNumber.Text.IsNotNullOrEmpty())
            {
                var deviceComponent = new DeviceComponent(_repositoryFactory);
                var deviceInCache = deviceComponent.SearchDeviceCache(d => d.DeviceNumber == txtSendFoodTitlesDeviceNumber.Text.ToInt32())
                    .FirstOrDefault();
                if (deviceInCache != null)
                {
                    var titles = txtSendFoodTitles.Lines.ToList();
                    var deviceInfo = deviceComponent.ConvertDeviceToDeviceInfo(new List<DtoDevice> { deviceInCache }).FirstOrDefault();
                    var communicationComponent = new CommunicationComponent(_repositoryFactory);
                    communicationComponent.CommunicationSendFunctionTitles(deviceInfo, titles);
                }
            }
        }

        private void btnDisableFoodTitles_Click(object sender, EventArgs e)
        {

            var deviceComponent = new DeviceComponent(_repositoryFactory);
            var deviceInCache = deviceComponent.SearchDeviceCache(d => d.DeviceNumber == txtSendFoodTitlesDeviceNumber.Text.ToInt32())
                .FirstOrDefault();
            if (deviceInCache != null)
            {
                var deviceInfo = deviceComponent.ConvertDeviceToDeviceInfo(new List<DtoDevice> { deviceInCache }).FirstOrDefault();
                var communicationComponent = new CommunicationComponent(_repositoryFactory);
                communicationComponent.CommunicationDisableFunctionTitles(deviceInfo);
            }

        }

        private void btnSelfPrintInfoClear_Click(object sender, EventArgs e)
        {
            if (chkSelfPrintInfoClearTitle2.Checked)
            {
                txtSelfPrintInfoHeaderTitle2.Text = string.Empty;
            }
            if (chkSelfPrintInfoClearTitle3.Checked)
            {
                txtSelfPrintInfoHeaderTitle3.Text = string.Empty;
            }
            if (chkSelfPrintInfoClearTitle4.Checked)
            {
                txtSelfPrintInfoHeaderTitle4.Text = string.Empty;
            }
            if (chkSelfPrintInfoClearManual.Checked)
            {
                txtManualFishString.Text = string.Empty;
            }

            if (chkSelfPrintInfoClearFood2.Checked)
            {
                txtSelfPrintInfoFoodTitle2.Text = string.Empty;
                txtSelfPrintInfoFoodDescription2.Text = string.Empty;
                txtSelfPrintInfoFishCount2.Text = string.Empty;
                txtSelfPrintInfoFoodType2.Text = string.Empty;
                txtSelfPrintInfoFoodPrice2.Text = string.Empty;
                chkSelfPrintInfoFoodIsAllowed2.Checked = false;
            }

            if (chkSelfPrintInfoClearFood3.Checked)
            {
                txtSelfPrintInfoFoodTitle3.Text = string.Empty;
                txtSelfPrintInfoFoodDescription3.Text = string.Empty;
                txtSelfPrintInfoFishCount3.Text = string.Empty;
                txtSelfPrintInfoFoodType3.Text = string.Empty;
                txtSelfPrintInfoFoodPrice3.Text = string.Empty;
                chkSelfPrintInfoFoodIsAllowed3.Checked = false;
            }

        }

        #region Timy

        private void btnAttendanceToolsSaveAttendanceByPublisher_Click(object sender, EventArgs e)
        {
            var deviceComponent = new DeviceComponent(_repositoryFactory);
            var devices = deviceComponent.SearchDeviceCache(r => r.IsHookActive).Take(txtAttendanceToolsDeviceCount.Text.ToInt32()).ToList();
            foreach (var dv in devices)
            {
                var thread = new Thread(() => AddAttendanceByPublisher(dv));
                Thread.Sleep(5);
                thread.Start();
            }
        }

        private void btnAttendanceToolsSaveAttendanceByComponent_Click(object sender, EventArgs e)
        {
            var deviceComponent = new DeviceComponent(_repositoryFactory);
            var devices = deviceComponent.SearchDeviceCache(r => r.IsHookActive).Take(txtAttendanceToolsDeviceCount.Text.ToInt32()).ToList();
            foreach (var dv in devices)
            {
                var thread = new Thread(() => AddAttendanceByComponent(dv));
                Thread.Sleep(5);
                thread.Start();
            }
        }

        private void AddAttendanceByPublisher(DtoDevice device)
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            var delay = txtAttendanceToolsDelay.Text.ToInt32();
            var attendanceCount = txtAttendanceToolsAttendanceCount.Text.ToInt32();
            for (var i = 0; i < attendanceCount; i++)
            {
                var attendance = new DtoAttendance
                {
                    EmployeeNumber = rnd.Next(100, 100000),
                    DeviceNumber = device.DeviceNumber,
                    CameraId = null,
                    ApplicationId = ApplicationTypeEnumeration.All,
                    AttendanceDateTime = DateTime.Now,//.AddSeconds(rnd.Next(0, 600) * -1),
                    AttendanceSource = AttendanceSourceEnumeration.Device,
                    DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                    IoType = DeviceIoTypeEnumeration.InputOutput,
                    IsInvalid = false,
                    IsSent = false,
                    RfCardNumber = null,
                    StatusCode = 0,
                    VerificationStyle = null,
                };
                Console.WriteLine($@"New Attendance E = {attendance.EmployeeNumber}, D= {device.DeviceNumber}");
                HardwareEventPublisher.Instance.PublishAttendance(attendance);
                Thread.Sleep(rnd.Next(1, delay));
            }
        }

        private void AddAttendanceByComponent(DtoDevice device)
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            var delay = txtAttendanceToolsDelay.Text.ToInt32();
            var attendanceCount = txtAttendanceToolsAttendanceCount.Text.ToInt32();
            for (var i = 0; i < attendanceCount; i++)
            {
                var attendanceComponent = new AttendanceComponent(_repositoryFactory);
                var attendance = new DtoAttendance
                {
                    EmployeeNumber = rnd.Next(100, 100000),
                    DeviceNumber = device.DeviceNumber,
                    CameraId = null,
                    ApplicationId = ApplicationTypeEnumeration.All,
                    AttendanceDateTime = DateTime.Now,//.AddSeconds(rnd.Next(0, 600) * -1),
                    AttendanceSource = AttendanceSourceEnumeration.Device,
                    DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                    IoType = DeviceIoTypeEnumeration.InputOutput,
                    IsInvalid = false,
                    IsSent = false,
                    RfCardNumber = null,
                    StatusCode = 0,
                    VerificationStyle = null,
                };
                Console.WriteLine($@"New Attendance E = {attendance.EmployeeNumber}, D= {device.DeviceNumber}");
                attendanceComponent.SaveAttendance(new List<DtoAttendance>() { attendance }, true, true, true);
                Thread.Sleep(rnd.Next(1, delay));
            }
        }

        private void btnTimySetPersTimezone_Click(object sender, EventArgs e)
        {
            if (!txtTimyDeviceNumber.Text.CanConvertToInt32())
            {
                MessageBox.Show("شماره دستگاه Timy را وارد کنید");
                return;
            }
            if (!txtEnrollId.Text.CanConvertToInt32())
            {
                MessageBox.Show("کد پرسنلی را وارد نمایید");
                return;
            }

            var deviceInCache = _deviceComponent.SearchDeviceCache
                (d => d.DeviceNumber == txtTimyDeviceNumber.Text.ToInt32()).FirstOrDefault();
            if (deviceInCache == null)
            {
                MessageBox.Show("شماره دستگاه معتبر نمی باشد");
                return;
            }

            FlushTimyDayGrid();
            FlushTimyWeekGrid();


            var deviceConverted = _deviceComponent.ConvertDeviceToDeviceInfo
                (new List<DtoDevice> { deviceInCache }).FirstOrDefault();
            var deviceCommands = TimyPushCommands.GetUserTimezoneCommand
                (deviceConverted, txtEnrollId.Text.ToInt64()
                    , cmbUserWeekzone.SelectedIndex + 1
                    , dtUserStartTime.Value, dtUserEndTime.Value
                    , 10, null, null, null, null);
            _deviceCommandComponent.Insert(deviceCommands);

        }


        private void btnTimyHoliday_Click(object sender, EventArgs e)
        {

            if (!txtTimyDeviceNumber.Text.CanConvertToInt32())
            {
                MessageBox.Show("شماره دستگاه Timy را وارد کنید");
                return;
            }

            var deviceInCache = _deviceComponent.SearchDeviceCache
                (d => d.DeviceNumber == txtTimyDeviceNumber.Text.ToInt32()).FirstOrDefault();
            if (deviceInCache == null)
            {
                MessageBox.Show("شماره دستگاه معتبر نمی باشد");
                return;
            }

            var deviceConverted = _deviceComponent.ConvertDeviceToDeviceInfo
                (new List<DtoDevice> { deviceInCache }).FirstOrDefault();
            var deviceCommands = TimyPushCommands.GetHolidayCommand
            (deviceConverted, _timyHolidays.ToList()
                , 10
                , null
                , null
                , null
                , null);
            _deviceCommandComponent.Insert(deviceCommands);

            
        }

        // ---------- Set For User tab ----------

        private void InitializeTimySetForUserTab()
        {
            dtUserStartTime.Value = DateTime.Now;
            dtUserEndTime.Value = DateTime.Now;
            RefreshUserWeekzoneItems();
        }

        /// <summary>
        /// Rebuilds the "1 - Office Hours" options offered by the user Weekzone combo from the
        /// WeekTimezoneGroups. Called whenever those groups are added, removed, renamed or reordered.
        /// </summary>
        private void RefreshUserWeekzoneItems()
        {
            if (cmbUserWeekzone == null) return;

            var previous = cmbUserWeekzone.SelectedValue as int?;

            var items = new List<TimyDayIndexItem>();
            foreach (var group in _timyWeekGroups)
                items.Add(new TimyDayIndexItem(group.DeviceIndex, $"{group.DeviceIndex} - {DisplayTitle(group.Title)}"));

            cmbUserWeekzone.DataSource = items;
            cmbUserWeekzone.DisplayMember = nameof(TimyDayIndexItem.Display);
            cmbUserWeekzone.ValueMember = nameof(TimyDayIndexItem.Value);

            if (previous.HasValue)
                cmbUserWeekzone.SelectedValue = previous.Value;
        }

        /// <summary>Collects the Set For User tab into a TimyUserAccess (defaults kept for the hidden fields).</summary>
        private TimyUserAccess BuildTimyUserAccess()
        {
            var weekzone = cmbUserWeekzone.SelectedValue is int selectedWeekzone ? selectedWeekzone : 0;

            return new TimyUserAccess
            {
                enrollid = txtEnrollId.Text.CanConvertToInt32() ? txtEnrollId.Text.ToInt32() : 0,
                weekzone = weekzone,
                starttime = dtUserStartTime.Value,
                endtime = dtUserEndTime.Value
            };
        }

        /// <summary>Restores the Set For User tab from a loaded TimyUserAccess.</summary>
        private void ApplyTimyUserAccess(TimyUserAccess userAccess)
        {
            if (userAccess == null) return;

            txtEnrollId.Text = userAccess.enrollid.ToString();

            // Make sure the options reflect the (already loaded) week groups before selecting.
            RefreshUserWeekzoneItems();
            cmbUserWeekzone.SelectedValue = userAccess.weekzone;

            dtUserStartTime.Value = ClampToDateTimePicker(userAccess.starttime, dtUserStartTime);
            dtUserEndTime.Value = ClampToDateTimePicker(userAccess.endtime, dtUserEndTime);
        }

        private static DateTime ClampToDateTimePicker(DateTime value, DateTimePicker picker)
        {
            if (value < picker.MinDate) return picker.MinDate;
            if (value > picker.MaxDate) return picker.MaxDate;
            return value;
        }

        // ---------- Holiday List tab ----------

        private readonly List<DtoTimyHoliday> _timyHolidays = new List<DtoTimyHoliday>();

        private void InitializeTimyHolidayTab()
        {
            RefreshTimyHolidayDayTimezoneItems();
            UpdateTimyHolidayDetailEnabled();
        }

        private DtoTimyHoliday SelectedTimyHoliday =>
            lstTimyHolidays.SelectedIndex >= 0 && lstTimyHolidays.SelectedIndex < _timyHolidays.Count
                ? _timyHolidays[lstTimyHolidays.SelectedIndex]
                : null;

        private void BtnTimyHolidayAdd_Click(object sender, EventArgs e)
        {
            var holiday = new DtoTimyHoliday
            {
                Title = "Holiday " + (_timyHolidays.Count + 1),
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.Date,
                DayTimezoneIndex = _timyDayGroups.Count > 0 ? _timyDayGroups.Min(g => g.DeviceIndex) : 0
            };
            _timyHolidays.Add(holiday);
            RefreshTimyHolidayList();
            lstTimyHolidays.SelectedIndex = _timyHolidays.Count - 1;
        }

        private void BtnTimyHolidayRemove_Click(object sender, EventArgs e)
        {
            var index = lstTimyHolidays.SelectedIndex;
            if (index < 0) return;

            _timyHolidays.RemoveAt(index);
            RefreshTimyHolidayList();
            if (_timyHolidays.Count > 0)
                lstTimyHolidays.SelectedIndex = Math.Min(index, _timyHolidays.Count - 1);
            else
                LoadTimyHolidayDetail();
        }

        private void LstTimyHolidays_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressTimyEvents) return;
            LoadTimyHolidayDetail();
        }

        private void TxtTimyHolidayTitle_TextChanged(object sender, EventArgs e)
        {
            if (_suppressTimyEvents) return;
            var holiday = SelectedTimyHoliday;
            if (holiday == null) return;
            holiday.Title = txtTimyHolidayTitle.Text;
            RefreshTimyHolidayList();
        }

        private void DtTimyHolidayStartDate_ValueChanged(object sender, EventArgs e)
        {
            if (_suppressTimyEvents) return;
            var holiday = SelectedTimyHoliday;
            if (holiday != null) holiday.StartDate = dtTimyHolidayStartDate.Value;
        }

        private void DtTimyHolidayEndDate_ValueChanged(object sender, EventArgs e)
        {
            if (_suppressTimyEvents) return;
            var holiday = SelectedTimyHoliday;
            if (holiday != null) holiday.EndDate = dtTimyHolidayEndDate.Value;
        }

        private void CmbTimyHolidayDayTimezone_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressTimyEvents) return;
            var holiday = SelectedTimyHoliday;
            if (holiday != null && cmbTimyHolidayDayTimezone.SelectedValue is int dayTimezoneIndex)
                holiday.DayTimezoneIndex = dayTimezoneIndex;
        }

        private void LoadTimyHolidayDetail()
        {
            var holiday = SelectedTimyHoliday;
            var wasSuppressed = _suppressTimyEvents;
            _suppressTimyEvents = true;
            if (holiday == null)
            {
                txtTimyHolidayTitle.Text = string.Empty;
                dtTimyHolidayStartDate.Value = ClampToDateTimePicker(DateTime.Now, dtTimyHolidayStartDate);
                dtTimyHolidayEndDate.Value = ClampToDateTimePicker(DateTime.Now, dtTimyHolidayEndDate);
                if (cmbTimyHolidayDayTimezone.Items.Count > 0) cmbTimyHolidayDayTimezone.SelectedIndex = 0;
            }
            else
            {
                txtTimyHolidayTitle.Text = holiday.Title ?? string.Empty;
                dtTimyHolidayStartDate.Value = ClampToDateTimePicker(holiday.StartDate, dtTimyHolidayStartDate);
                dtTimyHolidayEndDate.Value = ClampToDateTimePicker(holiday.EndDate, dtTimyHolidayEndDate);
                cmbTimyHolidayDayTimezone.SelectedValue = holiday.DayTimezoneIndex;
            }
            _suppressTimyEvents = wasSuppressed;
            UpdateTimyHolidayDetailEnabled();
        }

        private void RefreshTimyHolidayList()
        {
            var wasSuppressed = _suppressTimyEvents;
            _suppressTimyEvents = true;
            var selected = lstTimyHolidays.SelectedIndex;
            lstTimyHolidays.BeginUpdate();
            lstTimyHolidays.Items.Clear();
            for (var i = 0; i < _timyHolidays.Count; i++)
                lstTimyHolidays.Items.Add($"{i + 1} - {DisplayTitle(_timyHolidays[i].Title)}");
            if (selected >= 0 && selected < lstTimyHolidays.Items.Count)
                lstTimyHolidays.SelectedIndex = selected;
            lstTimyHolidays.EndUpdate();
            _suppressTimyEvents = wasSuppressed;
        }

        private void UpdateTimyHolidayDetailEnabled()
        {
            var enabled = SelectedTimyHoliday != null;
            txtTimyHolidayTitle.Enabled = enabled;
            dtTimyHolidayStartDate.Enabled = enabled;
            dtTimyHolidayEndDate.Enabled = enabled;
            cmbTimyHolidayDayTimezone.Enabled = enabled;
        }

        /// <summary>
        /// Rebuilds the DayTimezone options offered on the Holiday tab from the DayTimezoneGroups.
        /// Called whenever those groups are added, removed or renamed.
        /// </summary>
        private void RefreshTimyHolidayDayTimezoneItems()
        {
            if (cmbTimyHolidayDayTimezone == null) return;

            var wasSuppressed = _suppressTimyEvents;
            _suppressTimyEvents = true;

            var items = new List<TimyDayIndexItem>();
            foreach (var group in _timyDayGroups)
                items.Add(new TimyDayIndexItem(group.DeviceIndex, $"{group.DeviceIndex} - {DisplayTitle(group.Title)}"));

            cmbTimyHolidayDayTimezone.DataSource = items;
            cmbTimyHolidayDayTimezone.DisplayMember = nameof(TimyDayIndexItem.Display);
            cmbTimyHolidayDayTimezone.ValueMember = nameof(TimyDayIndexItem.Value);

            var holiday = SelectedTimyHoliday;
            if (holiday != null)
                cmbTimyHolidayDayTimezone.SelectedValue = holiday.DayTimezoneIndex;

            _suppressTimyEvents = wasSuppressed;
        }

        /// <summary>Restores the Holiday tab from a loaded holiday list.</summary>
        private void ApplyTimyHolidays(List<DtoTimyHoliday> holidays)
        {
            var wasSuppressed = _suppressTimyEvents;
            _suppressTimyEvents = true;
            _timyHolidays.Clear();
            if (holidays != null)
                foreach (var holiday in holidays)
                    _timyHolidays.Add(holiday);
            _suppressTimyEvents = wasSuppressed;

            RefreshTimyHolidayDayTimezoneItems();
            RefreshTimyHolidayList();

            if (_timyHolidays.Count > 0)
                lstTimyHolidays.SelectedIndex = 0;
            else
                LoadTimyHolidayDetail();
        }

        public class TimyUserAccess
        {
            public int enrollid { get; set; }
            public int weekzone { get; set; }
            public int weekzone2 { get; set; } = 0;
            public int weekzone3 { get; set; } = 0;
            public int weekzone4 { get; set; } = 0;
            public int group { get; set; } = 0;
            public DateTime starttime { get; set; }
            public DateTime endtime { get; set; }

        }

        #endregion

    }
}
