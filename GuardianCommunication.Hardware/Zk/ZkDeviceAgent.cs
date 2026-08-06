using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.Shared;
using GuardianCommunication.Hardware.Shared.Helpers;
using zkemkeeper;
using Timer = System.Timers.Timer;

namespace GuardianCommunication.Hardware.Zk
{
    public class ZkDeviceAgent : IDisposable
    {
        private readonly ZkAgentConfig _config;
        private readonly CZKEMClass _zkSdk = new CZKEMClass();
        private bool? _isDeviceConnected;
        private DateTime _lastDataReceiveTime = DateTime.Now;
        private Thread _pingThread;
        private CancellationTokenSource _pingCts;
        private Timer _lastDataCheckTimer;

        public bool IsDeviceConnected
        {
            get => _isDeviceConnected ?? false;
            set => _isDeviceConnected = value;
        }
        public DtoCommunicationDeviceData DeviceInfo { get; set; }


        public ZkDeviceAgent(DtoCommunicationDeviceData deviceInfo, ZkAgentConfig config)
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AgentCreate))
            {
                LoggingSystem.LogInfo("New Zk agent created", deviceInfo.DeviceNumber);
            }
            _config = config;
            DeviceInfo = deviceInfo;
            if (!string.IsNullOrEmpty(DeviceInfo.CommunicationPassword) && DeviceInfo.CommunicationPassword.CanConvertToInt32() && DeviceInfo.CommunicationPassword.ToInt32() > 0)
            {
                _zkSdk.SetCommPassword(DeviceInfo.CommunicationPassword.ToInt32());
            }
            if (DeviceInfo.OnlineMonitoringMode)
            {
                StartOnlineMonitoring("Agent Created");
                StartOnlineMonitoringThreadsAndTimers();
            }
        }

        private OperationResultEnumeration StartOnlineMonitoring(string reason)
        {
            var result = OperationResultEnumeration.CommunicationStatusSuccessful;
            try
            {
                if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AgentStartStopMonitoring))
                {
                    LoggingSystem.LogInfo("ZkDeviceAgent StartOnlineMonitoring method called", new
                    {
                        DeviceInfo.DeviceNumber,
                        Reason = reason,
                        DeviceInfo.Ip,
                    });
                }

                if (DeviceInfo.ConnectionTypeEnum != ConnectionTypeEnumeration.Ethernet || !DeviceInfo.TcpPort.HasValue)
                {
                    if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AgentStartStopMonitoring))
                    {
                        LoggingSystem.LogInfo($"Monitoring agent ZK is in not supported Connection Type state for Device {DeviceInfo.DeviceNumber}", new
                        {
                            DeviceInfo.DeviceNumber,
                            Reason = reason,
                            DeviceInfo.Ip,
                        });
                    }
                    return OperationResultEnumeration.CommunicationStatusNotSupport;
                }

                if (_config.IsNetworkPingActive
                    && !NetworkHelpers.PingHost(DeviceInfo.Ip, _config.PingTimeoutInMillisecond))
                {
                    if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AgentStartStopMonitoring))
                    {
                        LoggingSystem.LogInfo($"Monitoring agent ZK is in not supported Connection Type state for Device {DeviceInfo.DeviceNumber}", new
                        {
                            DeviceInfo.DeviceNumber,
                            Reason = reason,
                            DeviceInfo.Ip,
                        });
                    }
                    return OperationResultEnumeration.CommunicationStatusZkErrorOnResetOnlineMonitoringBecauseNoPing;
                }

                if (_zkSdk.Connect_Net(DeviceInfo.Ip, DeviceInfo.TcpPort.Value))
                {
                    IsDeviceConnected = true;
                    HardwareEventPublisher.Instance.PublishDeviceConnectionStatusChanged(new List<DtoDeviceConnectionStatus>
                    {
                        new DtoDeviceConnectionStatus
                        {
                            DeviceNumber = DeviceInfo.DeviceNumber,
                            IsConnected = true
                        }
                    });
                    _zkSdk.OnDisConnected += ZkSdkOnDisconnected;
                    if (!DeviceInfo.JustDoorControl)
                    {
                        if (_zkSdk.RegEvent(DeviceInfo.DeviceNumber, 65535))
                        {
                            _zkSdk.OnAttTransactionEx += ZkSdkOnOnAttTransactionEx;
                            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AgentStartStopMonitoring))
                            {
                                LoggingSystem.LogInfo(
                                    $"Register Attendance Event Successfully For Device {DeviceInfo.DeviceNumber}",
                                    new
                                    {
                                        DeviceInfo.DeviceNumber,
                                        Reason = reason,
                                        DeviceInfo.Ip,
                                    });
                            }

                        }
                        else
                        {
                            var errorCode = 0;
                            _zkSdk.GetLastError(ref errorCode);
                            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AgentStartStopMonitoring))
                            {
                                LoggingSystem.LogInfo(
                                    $"Error {errorCode} On RegEvent method for Device {DeviceInfo.DeviceNumber}",
                                    new
                                    {
                                        DeviceInfo.DeviceNumber,
                                        Reason = reason,
                                        DeviceInfo.Ip,
                                    });
                            }
                            return OperationResultEnumeration.CommunicationStatusZkErrorOnRegEvent;
                        }
                    }
                }
                else
                {
                    IsDeviceConnected = false;
                    HardwareEventPublisher.Instance.PublishDeviceConnectionStatusChanged(new List<DtoDeviceConnectionStatus>
                    {
                        new DtoDeviceConnectionStatus
                        {
                            DeviceNumber = DeviceInfo.DeviceNumber,
                            IsConnected = false
                        }
                    });
                    var errorCode = 0;
                    _zkSdk.GetLastError(ref errorCode);
                    if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AgentStartStopMonitoring))
                    {
                        LoggingSystem.LogInfo(
                            $"Error {errorCode} on connecting to device {DeviceInfo.DeviceNumber}", new
                            {
                                DeviceInfo.DeviceNumber,
                                Reason = reason,
                                DeviceInfo.Ip,
                            });
                    }
                    return OperationResultEnumeration.CommunicationStatusCannotConnect;
                }

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, $"Exception on StartOnlineMonitoring {DeviceInfo.DeviceNumber}", new
                {
                    DeviceInfo.DeviceNumber,
                    Reason = reason,
                    DeviceInfo.Ip,
                });
                return OperationResultEnumeration.CommunicationStatusUnknownError;
            }
            return result;
        }

        private OperationResultEnumeration StopOnlineMonitoring(string reason)
        {
            try
            {
                if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AgentStartStopMonitoring))
                {
                    LoggingSystem.LogInfo($"ZkDeviceAgent StopOnlineMonitoring method called {DeviceInfo.DeviceNumber}", new
                    {
                        DeviceInfo.DeviceNumber,
                        Reason = reason,
                        DeviceInfo.Ip,
                    });
                }
                _zkSdk.Disconnect();
                _zkSdk.OnDisConnected -= ZkSdkOnDisconnected;
                if (!DeviceInfo.JustDoorControl)
                {
                    _zkSdk.OnAttTransactionEx -= ZkSdkOnOnAttTransactionEx;
                }
                if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AgentStartStopMonitoring))
                {
                    LoggingSystem.LogInfo($"Monitoring agent ZK device STOPPED For Device {DeviceInfo.DeviceNumber}", new
                    {
                        DeviceInfo.DeviceNumber,
                        Reason = reason,
                        DeviceInfo.Ip,
                    });
                }

                return OperationResultEnumeration.CommunicationStatusSuccessful;
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, $"Exception on StopOnlineMonitoring {DeviceInfo.DeviceNumber}", new
                {
                    DeviceInfo.DeviceNumber,
                    Reason = reason,
                    DeviceInfo.Ip,
                });
                return OperationResultEnumeration.CommunicationStatusUnknownError;
            }

        }

        private readonly object _lockResetOnlineMonitoring = new object();
        public OperationResultEnumeration ResetOnlineMonitoring(string reason)
        {
            if (Monitor.TryEnter(_lockResetOnlineMonitoring))
            {
                try
                {
                    var res = StopOnlineMonitoring(reason);
                    if (res != OperationResultEnumeration.CommunicationStatusSuccessful)
                    {
                        return res;
                    }
                    return StartOnlineMonitoring(reason);
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp);
                    return OperationResultEnumeration.CommunicationStatusUnknownError;
                }
                finally
                {
                    Monitor.Exit(_lockResetOnlineMonitoring);
                }
            }
            else
            {
                if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AgentStartStopMonitoring))
                {
                    LoggingSystem.LogInfo($"ResetOnlineMonitoring skipped because of lock is taken for device {DeviceInfo}", "ZkDeviceAgent Reset online monitoring");
                }

                return OperationResultEnumeration.CommunicationStatusZkErrorOnResetOnlineMonitoringBecauseOfLockIsTaken;
            }
        }


        public void OpenDoor(int timeoutInSecond)
        {
            if (IsDeviceConnected == false)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);

            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AgentOpenDoor))
            {
                LoggingSystem.LogInfo("ZkDeviceAgent open door", DeviceInfo.DeviceNumber);
            }

            var errorCode = 0;
            if (!_zkSdk.ACUnlock(DeviceInfo.DeviceNumber, timeoutInSecond))
            {
                _zkSdk.GetLastError(ref errorCode);
                throw new OperationCannotBeDoneException(
                    DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
            }

            if (AppConfigs.ZkOpenDoorDelay > 0)
            {
                Thread.Sleep(AppConfigs.ZkOpenDoorDelay);
                if (!_zkSdk.ACUnlock(DeviceInfo.DeviceNumber, timeoutInSecond))
                {
                    _zkSdk.GetLastError(ref errorCode);
                    throw new OperationCannotBeDoneException(
                        DeviceSharedHelperMethods.MapToOperationResult(errorCode, DeviceInfo));
                }
            }
        }

        private void StartOnlineMonitoringThreadsAndTimers()
        {
            if (!DeviceInfo.JustDoorControl)
            {
                if (_lastDataCheckTimer == null)
                {
                    _lastDataCheckTimer = new Timer(new TimeSpan(0, 0,
                        _config.TimerCheckLastDataIntervalInMinutes, 0).TotalMilliseconds);
                    _lastDataCheckTimer.Elapsed += LastDataCheckTimerElapsed;
                }
                _lastDataCheckTimer.Start();
            }

            if (_config.IsNetworkPingActive && _pingThread == null)
            {
                _pingCts = new CancellationTokenSource();
                _pingThread = new Thread(PingOnlineDevice) { IsBackground = true };
                _pingThread.Start();
            }

        }

        private void StopOnlineMonitoringThreadsAndTimers()
        {
            if (!DeviceInfo.JustDoorControl)
            {
                if (_lastDataCheckTimer != null)
                {
                    _lastDataCheckTimer.Stop();
                    _lastDataCheckTimer.Elapsed -= LastDataCheckTimerElapsed;
                    _lastDataCheckTimer.Dispose();
                    _lastDataCheckTimer = null;
                }
            }

            if (_pingThread != null)
            {
                // Cooperative shutdown (replaces Thread.Abort): signal cancellation, wake the
                // ping wait immediately, then join with a bounded timeout so Dispose can never hang.
                //TODO: Ask about it
                _pingCts?.Cancel();
                if (!_pingThread.Join(5000)
                    && AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AgentStartStopMonitoring))
                {
                    LoggingSystem.LogInfo(
                        "ZkDeviceAgent ping thread did not stop within timeout; continuing (background thread)",
                        DeviceInfo.DeviceNumber);
                }
                _pingCts?.Dispose();
                _pingCts = null;
                _pingThread = null;
            }

        }

        private void ZkSdkOnOnAttTransactionEx(string enrollNumber, int isInvalid, int attState, int verifyMethod, int year, int month, int day, int hour, int minute, int second, int workCode)
        {
            if (DeviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
            {
                return;
            }

            _lastDataReceiveTime = DateTime.Now;
            var status = attState & 0x7F;
            var isInvalidValue = false;
            if (DeviceInfo.HasAttendanceValidationCheck)
            {
                isInvalidValue = isInvalid != 1;
            }
            var attendance = new DtoAttendance
            {
                EmployeeNumber = enrollNumber.ToInt64(),
                VerificationStyle = (int)ZkUtils.GetVerificationStyle(verifyMethod),
                DeviceNumber = DeviceInfo.DeviceNumber,
                CameraId = null,
                AttendanceDateTime = new DateTime(year, month, day, hour, minute, second),
                AttendanceSource = AttendanceSourceEnumeration.Device,
                DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.ZkOnlineMonitoring,
                StatusCode = status,
                // به صورت پیش فرض برای دستگاه های کنترل تردد آنلاین در ثورتی که ذکر شده بود برای دستگاه صحت
                // تردد بر اساس رویداد گذر باید چک شود، می بایست تردد به صورت پچیش فرض غیر مجاز فرض گردد
                IsInvalid = isInvalidValue,
                RfCardNumber = null,
            };
            HardwareEventPublisher.Instance.PublishAttendance(attendance);
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AgentLog))
            {
                LoggingSystem.LogInfo("ZkDeviceAgent Real time data", attendance);
            }
        }

        private void ZkSdkOnDisconnected()
        {
            try
            {
                if (_config.IsNetworkPingActive && NetworkHelpers.PingHost(DeviceInfo.Ip, _config.PingTimeoutInMillisecond))
                {
                    ResetOnlineMonitoring("Disconnect Event from SDK");
                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "ZkDeviceAgent Error ZkDeviceAgent.ZkSdkOnDisConnected");
            }
        }



        #region Timers & Thread


        private readonly object _lockVerifyStatus = new object();
        private void LastDataCheckTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (Monitor.TryEnter(_lockVerifyStatus))
            {
                try
                {
                    if (Math.Abs(_lastDataReceiveTime.Subtract(DateTime.Now).TotalMinutes) >= _config.IntervalFromLastDataToReset)
                    {
                        if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AgentLastDataTimer))
                        {
                            LoggingSystem.LogInfo("ZkDeviceAgent Monitoring rested because of last data ", DeviceInfo.DeviceNumber);
                        }
                        ResetOnlineMonitoring("No data received for specific time");
                    }
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp);
                }
                finally
                {
                    Monitor.Exit(_lockVerifyStatus);
                }
            }
            else
            {
                if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AgentLastDataTimer))
                {
                    LoggingSystem.LogInfo("ZkDeviceAgent LastDataCheckTimerElapsed skipped because of lock is taken", DeviceInfo.DeviceNumber);
                }
            }
        }



        private bool _isPingSuccessful = true;
        private void PingOnlineDevice()
        {
            var token = _pingCts.Token;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        if (NetworkHelpers.PingHost(DeviceInfo.Ip, _config.PingTimeoutInMillisecond))
                        {
                            if (!_isPingSuccessful)
                            {
                                // در صورتی که پینگ دستگاه رفته بود و پینگ مجددا وصل شد می بایست
                                // دستگاه را مجددا به مانیتورینگ وصل کنیم
                                if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AgentPingReset))
                                {
                                    LoggingSystem.LogInfo("ZkDeviceAgent Monitoring reset because of ping", DeviceInfo.DeviceNumber);
                                }
                                // Cancelable wait: wakes immediately on stop instead of sleeping the full interval.
                                if (token.WaitHandle.WaitOne(_config.WaitAfterPingIsConnectedAgainInSecond * 1000))
                                {
                                    break;
                                }
                                ResetOnlineMonitoring("Ping");
                            }
                            _isPingSuccessful = true;
                        }
                        else
                        {
                            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.AgentPingNotSuccess))
                            {
                                LoggingSystem.LogInfo("ZkDeviceAgent Ping not success", DeviceInfo.DeviceNumber);
                            }
                            _isPingSuccessful = false;
                            IsDeviceConnected = false;
                        }
                    }
                    catch
                    {
                        _isPingSuccessful = false;
                        IsDeviceConnected = false;
                    }
                    if (_config.SleepAfterPingInSecond > 0)
                    {
                        // Cancelable wait so a stop request ends the loop promptly.
                        if (token.WaitHandle.WaitOne(_config.SleepAfterPingInSecond * 1000))
                        {
                            break;
                        }
                    }
                }
            }
            catch
            {
                // Safety net: keep an unexpected exception from tearing down the background thread.
            }
        }

        #endregion


        #region Implementation of IDisposable

        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                // Managed/COM teardown only on explicit Dispose — never on the finalizer thread.
                StopOnlineMonitoringThreadsAndTimers();
                StopOnlineMonitoring("Dispose");
            }

            _disposed = true;
        }

        #endregion


    }
}