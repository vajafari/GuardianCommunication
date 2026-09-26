using GuardianCommunication.Business.Component;
using GuardianCommunication.Business.LiveModule;
using GuardianCommunication.Business.Tasks;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Data.Repository;
using GuardianCommunication.Hardware.Shared;
using GuardianCommunication.Hardware.Suprema;
using GuardianCommunication.Hardware.Suprema.SupremaConcepts.V1;
using GuardianCommunication.Hardware.Suprema.SupremaConcepts.V2;
using GuardianCommunication.Hardware.Timy;
using GuardianCommunication.Hardware.Virdi;
using GuardianCommunication.Hardware.Zk;
using GuardianCommunication.Service;
using GuardianCommunication.Service.WCF;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.SearchDataWrapper;
using GuardianCommunication.Shared.SharedSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading;
using System.Windows.Forms;

namespace TestHost
{
    public partial class FormHost : Form
    {
        private readonly RepositoryFactory _repositoryFactory;
        private readonly DeviceCommandComponent _deviceCommandComponent;
        private readonly DeviceComponent _deviceComponent;
        private readonly AttendanceComponent _attendanceComponent;
        private readonly SystemConfigComponent _systemConfigComponent;
        private ServiceHost _hardwareHost;

        private AutoCollectZkTask _autoCollectZkTask;
        private AutoCollectTimyTask _autoCollectTimyTask;
        private AutoCollectSupremaSdk1Task _autoCollectSupremaSdk1Task;
        private AutoCollectSupremaSdk2Task _autoCollectSupremaSdk2Task;
        private AutoCollectVirdiTask _autoCollectVirdiTask;
        private AttendanceSendToGuardianTask _attendanceSendToGuardianTask;
        private AttendanceHookTask _attendanceHookTask;
        private OnlineDeviceTask _onlineDeviceTask;

        public FormHost()
        {
            _repositoryFactory = new RepositoryFactory();
            LoggingSystem.Initialize(_repositoryFactory);
            _deviceCommandComponent = new DeviceCommandComponent(_repositoryFactory);
            _deviceComponent = new DeviceComponent(_repositoryFactory);
            _attendanceComponent = new AttendanceComponent(_repositoryFactory);
            _systemConfigComponent = new SystemConfigComponent(_repositoryFactory);
            InitializeComponent();
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
                foreach (var endpoint in _hardwareHost.Description.Endpoints)
                {
                    endpoint.Behaviors.Add(new CamelCaseTolerantJsonEndpointBehavior());
                }
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
                        _autoCollectZkTask.Process();
                        break;
                    case 4:
                        _autoCollectTimyTask.Process();
                        break;
                    case 5:
                        _attendanceHookTask.Process();
                        break;
                    case 6:
                        _attendanceSendToGuardianTask.Process();
                        break;
                    case 7:
                        _onlineDeviceTask.Process();
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
                MessageBox.Show(exp.GetFullExceptionMessage());
            }


        }

        private void StartTasks(DtoSystemConfig systemConfig)
        {
            _attendanceSendToGuardianTask =
                new AttendanceSendToGuardianTask(
                    TimeSpan.FromMinutes(systemConfig.AttendanceSendToGuardianTimerInterval));
            _attendanceHookTask = new AttendanceHookTask(TimeSpan.FromMinutes(systemConfig.AttendanceHookTimerInterval));
            _onlineDeviceTask = new OnlineDeviceTask(TimeSpan.FromSeconds(systemConfig.OnlineDeviceTimerInterval));

            var allTasks = new List<TimedBaseTask>
            {
                _attendanceSendToGuardianTask,
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
                if (ApplicationEmbeddedInfo.SupremaProducerVersions.Contains(SdkVersionEnumeration.SdkVersion1))
                {
                    _autoCollectSupremaSdk1Task =
                        new AutoCollectSupremaSdk1Task(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval));
                    allTasks.Add(_autoCollectSupremaSdk1Task);
                }
                if (ApplicationEmbeddedInfo.SupremaProducerVersions.Contains(SdkVersionEnumeration.SdkVersion2))
                {
                    _autoCollectSupremaSdk2Task =
                        new AutoCollectSupremaSdk2Task(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval));
                    allTasks.Add(_autoCollectSupremaSdk2Task);
                }
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Timy))
            {
                _autoCollectTimyTask =
                    new AutoCollectTimyTask(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval));
                allTasks.Add(_autoCollectTimyTask);
            }
            if (ApplicationEmbeddedInfo.ActiveProducers.HasFlag(ProducerEnumeration.Virdi))
            {
                _autoCollectVirdiTask =
                    new AutoCollectVirdiTask(TimeSpan.FromMinutes(systemConfig.AutomaticCollectAttendanceTimerInterval));
                allTasks.Add(_autoCollectVirdiTask);
            }

            TaskManager.Instance.SetTimedBaseTaskList(allTasks.ToArray());
            TaskManager.Instance.SetContinuousTasksList();

        }

        private void StartAllHardwareServers(DtoSystemConfig systemConfig)
        {
            var deviceComponent = new DeviceComponent(_repositoryFactory);
            var allDevices = deviceComponent.SearchDevice(new PagingData<DeviceFilter, DeviceSortEnumeration>());

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
                            LoggingSystem.LogError("Cannot initialize SupremaSK1", "Cannot initialize SupremaSK1");
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
                        StartDelayInSecond = ServiceConstants.ServerDelayStart,
                        MaxVisibleLightImageSizeHeight = systemConfig.VirdiMaxVisibleLightImageSizeHeight,
                        MaxVisibleLightImageSizeInKb = systemConfig.VirdiMaxVisibleLightImageSizeInKb,
                        MaxVisibleLightImageSizeWidth = systemConfig.VirdiMaxVisibleLightImageSizeWidth,
                    }
                        , _deviceCommandComponent.GetUnsentCommandsForEachDevice
                        , _attendanceComponent.ProcessServerMatchEvent
                        , allDevices
                    );
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on start VirdiServer");
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
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on Stop TimyServer");
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


        private void btnTest_Click(object sender, EventArgs e)
        {
            var result1 = _deviceCommandComponent.GetUnsentCommandsCountByDeviceSerialNumberForEachDevice(new DeviceNotSentCommandsFilter
            {
                Producer = ProducerEnumeration.Zk,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                DeviceSerialNumbers = new List<string> { "1", "2", "3" }
            });
            var result2 = _deviceCommandComponent.GetUnsentCommandsCountByDeviceNumberForEachDevice(new DeviceNotSentCommandsFilter
            {
                Producer = ProducerEnumeration.Zk,
                SdkVersion = SdkVersionEnumeration.SdkVersion1,
                DeviceNumbers = new List<int> { 1, 2, 3 }
            });
        }

        private void btnDoStopProcess_Click(object sender, EventArgs e)
        {
            DoStopProcess();
        }

        #region Attendance Tools

        private void btnAttendanceToolsSaveAttendanceByPublisher_Click(object sender, EventArgs e)
        {
            var devices = _deviceComponent.SearchDevice(new PagingData<DeviceFilter, DeviceSortEnumeration>())
                .Take(txtAttendanceToolsDeviceCount.Text.ToInt32()).ToList();
            foreach (var dv in devices)
            {
                var thread = new Thread(() => AddAttendanceByPublisher(dv));
                Thread.Sleep(5);
                thread.Start();
            }
        }

        private void btnAttendanceToolsSaveAttendanceByComponent_Click(object sender, EventArgs e)
        {
            var devices = _deviceComponent.SearchDevice(new PagingData<DeviceFilter, DeviceSortEnumeration>())
                .Take(txtAttendanceToolsDeviceCount.Text.ToInt32()).ToList();
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
                var attendance = BuildTestAttendance(device, rnd);
                Console.WriteLine($@"New Attendance U = {attendance.UserIdOnDevice}, D= {device.DeviceNumber}");
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
                var attendance = BuildTestAttendance(device, rnd);
                Console.WriteLine($@"New Attendance U = {attendance.UserIdOnDevice}, D= {device.DeviceNumber}");
                _attendanceComponent.SaveAttendance(new List<DtoAttendance> { attendance }, true, true);
                Thread.Sleep(rnd.Next(1, delay));
            }
        }

        // Minimal attendance shaped only from fields the current DtoAttendance actually exposes,
        // enough to exercise the hook/save pipeline for manual testing.
        private static DtoAttendance BuildTestAttendance(DtoDevice device, Random rnd)
        {
            return new DtoAttendance
            {
                UserIdOnDevice = rnd.Next(100, 100000),
                DeviceId = device.Id,
                LocationId = device.LocationId,
                ModuleId = device.ModuleId,
                AttendanceDateTime = DateTime.Now,
                AttendanceSource = AttendanceSourceEnumeration.Device,
                DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                IoType = DeviceIoTypeEnumeration.InputOutput,
            };
        }

        #endregion

    }
}
