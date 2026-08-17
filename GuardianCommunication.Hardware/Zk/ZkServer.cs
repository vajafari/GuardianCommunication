using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using AccessControl.TimeHandling;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.Shared;
using GuardianCommunication.Hardware.Zk.ZkConcepts;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.Dto.Communication.Shared.CommunicationModels;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.HardwareDefinition;
using GuardianCommunication.Shared.SharedSettings;
using Timer = System.Timers.Timer;

namespace GuardianCommunication.Hardware.Zk
{
    [SuppressMessage("ReSharper", "CommentTypo")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("ReSharper", "InvertIf")]
    public class ZkServer : IDisposable
    {
        private const int MaxBufferSize = 1024 * 1024 * 2;
        private Func<DeviceNotSentCommandsFilter, List<DtoDeviceUnsentCommand>> _actionToGetCommands;
        private Func<DeviceNotSentCommandsFilter, List<DtoUnsentCommandCountByDeviceSerialNumber>> _actionToGetCommandsCountByDeviceSerialNumber;
        private int _maxCommandCount = ServiceConstants.HardwareServiceMaxZkCommands;
        private ZkPushConfig _pushConfig;
        private readonly Semaphore _threadControlSemaphore = new Semaphore
            (AppConfigs.SimultaneousZkServerThreadsCount, AppConfigs.SimultaneousZkServerThreadsCount);
        private Timer _getCommandTimers;
        private readonly ConcurrentDictionary<string, int> _commandCountDictionary = new ConcurrentDictionary<string, int>();

        #region Singleton

        public static ZkServer Instance { get; }

        private ZkServer()
        {
        }

        static ZkServer()
        {
            Instance = new ZkServer();
        }

        #endregion


        public void StartZkServer(
              ZkPushConfig pushConfig
            , Func<DeviceNotSentCommandsFilter, List<DtoDeviceUnsentCommand>> actionToGetCommands
            , Func<DeviceNotSentCommandsFilter, List<DtoUnsentCommandCountByDeviceSerialNumber>> actionToGetCommandsCountByDeviceSerialNumber
            , List<DtoDevice> deviceInfos)
        {
            _actionToGetCommands = actionToGetCommands;
            _actionToGetCommandsCountByDeviceSerialNumber = actionToGetCommandsCountByDeviceSerialNumber;

            LoggingSystem.LogInfo("ZKServer Starting", new
            {
                PushConfig = pushConfig
            });
            _pushConfig = pushConfig;
            _maxCommandCount = NumericHelper.Min(_pushConfig.MaxZkCommandCount, ServiceConstants.HardwareServiceMaxZkCommands);
            _getCommandTimers = new Timer(pushConfig.GetCommandTimerIntervalInMillisecond);
            _getCommandTimers.Elapsed += GetCommandTimersOnElapsed;
            _getCommandTimers.Start();
            var thread = new Thread(() => DoStartServerProcess(deviceInfos)) { IsBackground = true };
            thread.Start();
        }

        private void DoStartServerProcess(List<DtoDevice> deviceInfos)
        {
            SetDeviceOnPushModeList(deviceInfos);
            var thread = new Thread(StartPushListening) { IsBackground = true };
            thread.Start();
        }

        public void StopServer()
        {
            Dispose(true);
        }

        public List<Guid> GetConnectedDeviceNumbers()
        {
            return _pushDevicesConnectionInfo.Values
                .Where(d => Math.Abs(d.ConnectionDateTimeUtc.Subtract(DateTime.UtcNow).TotalSeconds) <=
                            _pushConfig.IntervalForConsiderDeviceOnlineInSecond)
                .Select(d => d.DeviceId).ToList();
        }


        #region PUSH


        public const string ServerVersion = "2.2.14";
        public const string PushProtocolVersion = "2.4.1";
        public const string PushOptionsFlag = "1";


        private TcpListener _tcpListener;
        private bool _listening;
        private readonly List<DtoDevice> _pushDevices = new List<DtoDevice>();
        private readonly ConcurrentDictionary<Guid, DeviceConnectionInfo> _pushDevicesConnectionInfo
            = new ConcurrentDictionary<Guid, DeviceConnectionInfo>();

        public void SetDeviceOnPushModeList(List<DtoDevice> deviceInfos)
        {
            if (deviceInfos.IsCollectionNullOrEmpty())
            {
                return;
            }

            var pushDeviceInfos = deviceInfos.Where
                (row => row.ProducerNumber == ProducerEnumeration.Zk
                        && row.ConnectionMode == DeviceConnectionModeEnumeration.Push).ToList();
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.ServerSetDeviceList))
            {
                LoggingSystem.LogInfo("ZKServer Set push device list", pushDeviceInfos);
            }

            lock (_pushDevices)
            {
                try
                {
                    _pushDevices.Clear();
                    _pushDevices.AddRange(pushDeviceInfos);
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp
                        , "ZkServer Error on SetDeviceOnPushModeList");
                }
            }

        }

        /// <summary>
        /// Start listening
        /// </summary>
        public void StartPushListening()
        {
            try
            {
                if (_tcpListener == null)
                {
                    _tcpListener = new TcpListener(IPAddress.Parse(_pushConfig.PushServerIp), _pushConfig.PushServerPort);
                }
                _tcpListener.Start();
                _listening = true;
                LoggingSystem.LogInfo("ZkServer Push listener started", _pushConfig);
                while (_listening)
                {
                    _threadControlSemaphore.WaitOne();
                    var handedOff = false;
                    try
                    {
                        var deviceSocket = _tcpListener.AcceptSocket();
                        var socketDataProcessThread =
                            new Thread(() => ProcessDeviceSocketAnalysis(deviceSocket)) { IsBackground = true };
                        socketDataProcessThread.Start();
                        handedOff = true; // the processing thread now owns releasing the permit (in its finally)
                        Thread.Sleep(_pushConfig.SleepBetweenSocketsInMillisecond);
                    }
                    catch (Exception exp)
                    {
                        // Accept/thread-start failed before hand-off: release the permit we acquired so it
                        // isn't leaked (otherwise the server eventually blocks on WaitOne and stops accepting).
                        if (!handedOff)
                        {
                            _threadControlSemaphore.Release();
                        }
                        LoggingSystem.LogError(exp, "ZkServer Error on starting push listener");
                    }
                }

                _tcpListener.Stop();
            }
            catch (Exception exp)
            {
                _listening = false;
                LoggingSystem.LogError(exp, $"ZkServer Error on listening to the IP {_pushConfig.PushServerIp} and port {_pushConfig.PushServerPort}");
            }
        }

        /// <summary>
        /// stop listening
        /// </summary>
        public void StopPushListening()
        {
            LoggingSystem.LogInfo("ZkServer StopPushListening is called");
            if (_listening)
            {
                _listening = false;
                _tcpListener.Stop();
            }
            if (_getCommandTimers != null)
            {
                _getCommandTimers.Stop();
                _getCommandTimers.Elapsed -= GetCommandTimersOnElapsed;
                _getCommandTimers.Dispose();
                _getCommandTimers = null;
            }
        }



        private readonly object _lockGetCommandObject = new object();
        private void GetCommandTimersOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.LogGetCommandTimersElapsed))
            {
                LoggingSystem.LogInfo("GetCommandTimersOnElapsed Process is calling");
            }
            if (Monitor.TryEnter(_lockGetCommandObject))
            {
                try
                {
                    List<string> deviceSerialNumbers;
                    lock (_pushDevices)
                    {
                        deviceSerialNumbers = _pushDevices.Where(d => d.SerialNumber.IsNotNullOrEmpty())
                            .Select(d => d.SerialNumber).ToList();
                    }
                    var commandInfoPure = _actionToGetCommandsCountByDeviceSerialNumber(new DeviceNotSentCommandsFilter
                    {
                        Producer = ProducerEnumeration.Zk,
                        SdkVersion = SdkVersionEnumeration.SdkVersion1,
                        DeviceSerialNumbers = deviceSerialNumbers,
                    });

                    //_commandCountDictionary.Clear();
                    foreach (var currentDeviceCommandInfo in commandInfoPure)
                    {
                        _commandCountDictionary[currentDeviceCommandInfo.DeviceSerialNumber]
                            = currentDeviceCommandInfo.CommandCount;
                    }
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp);
                }
                finally
                {
                    Monitor.Exit(_lockGetCommandObject);
                }
            }
        }

        private void ProcessDeviceSocketAnalysis(Socket deviceSocket)
        {
            string bufferString = null;
            try
            {
                //Old method
                deviceSocket.ReceiveBufferSize = MaxBufferSize;
                deviceSocket.SendBufferSize = MaxBufferSize;
                Thread.Sleep(_pushConfig.WaitForSocketData);
                if (deviceSocket.Available <= 0)
                    return;
                var receivedBytes = new byte[MaxBufferSize];
                deviceSocket.Receive(receivedBytes);
                bufferString = Encoding.ASCII.GetString(receivedBytes).TrimEnd().TrimEnd('\0');

                if (bufferString.IndexOfEx("cdata?") > 0)
                {
                    if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.ServerPushBufferTextDataFromDevice))
                    {
                        LoggingSystem.LogInfo(bufferString, "ZkServer Analysis is called");
                    }
                    ProcessDataFromDevice(bufferString, receivedBytes, deviceSocket);
                }
                else if (bufferString.IndexOfEx("getrequest?") > 0)
                {
                    if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.ServerPushBufferTextGetRequest))
                    {
                        LoggingSystem.LogInfo(bufferString, "ZkServer Get Request is called");
                    }
                    ProcessGetRequest(bufferString, deviceSocket);
                }
                else if (bufferString.IndexOfEx("devicecmd?") > 0)
                {
                    if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.ServerPushBufferTextCommandResponse))
                    {
                        LoggingSystem.LogInfo(bufferString, "ZkServer Process Device Command Response is called");
                    }
                    ProcessDeviceCommandResponse(bufferString, deviceSocket);
                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "ZkServer Error on ZK Analysis method", new
                {
                    LenghtOfString = bufferString?.Length ?? 0,
                    Message = AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.ServerLogWholeMessageOnException) ? bufferString : string.Empty
                });
            }
            finally
            {
                try
                {
                    _threadControlSemaphore.Release();
                    deviceSocket.Close();
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "ZkServer Error on ZK server closeing device socket");
                }
            }
        }

        private void ProcessDataFromDevice(string bufferString, byte[] receivedBytes, Socket deviceSocket)
        {
            //var bufferString = Encoding.ASCII.GetString(receivedBytes).TrimEnd().TrimEnd('\0');
            var serialNumber = GetValueByNameInPushHeader(bufferString, "SN");
            const string responseCode = "200 OK";
            const string responseString = "OK";
            if (bufferString.Substring(0, 3) == "GET") // iclock option
            {

                if (bufferString.IndexOfEx("options=all") > 0)
                {
                    var deviceInList = GetDeviceBySerialNumber(serialNumber);
                    SendDeviceConfig(deviceInList, deviceSocket);
                    if (deviceInList != null)
                    {
                        _pushDevicesConnectionInfo[deviceInList.Id] = new DeviceConnectionInfo
                        {
                            DeviceId = deviceInList.Id,
                            DeviceNumber = deviceInList.DeviceNumber,
                            ConnectionDateTimeUtc = DateTime.UtcNow,
                        };
                    }
                }
                else
                {
                    SendDataToDevice("400 Bad Request", "Unknown Command", deviceSocket);
                }
            }
            else if (bufferString.Substring(0, 4) == "POST")
            {
                // Only PUSH SDK Ver 2.0.1 (In Version 1.0 String for AttLog have diferent format, example: CHECK LOG: stamp=392232960 1       2018-03-14 17:39:00     0       0       0       1)
                //table=ATTLOG
                if (bufferString.IndexOfEx("Stamp", 1) > 0
                    && bufferString.IndexOfEx("OPERLOG", 1) < 0
                    && bufferString.IndexOfEx("ATTLOG", 1) > 0
                    && bufferString.IndexOfEx("OPLOG", 1) < 0) // Upload AttLog
                {

                    if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.ServerPushLogAttendanceString))
                    {
                        LoggingSystem.LogInfo("ZkPushServer attendance string", bufferString);
                    }
                    var deviceSerialNumberNotPure = bufferString.Substring(bufferString.IndexOfEx("SN=") + 3);
                    var deviceSerialNumber = deviceSerialNumberNotPure.Split('&')[0];
                    var attendanceIndex = bufferString.IndexOfEx("\r\n\r\n", 1);
                    var attendanceString = bufferString.Substring(attendanceIndex + 4);
                    var deviceInList = GetDeviceBySerialNumber(deviceSerialNumber);
                    if (deviceInList == null
                        || deviceInList.DeviceSettings.DontSaveAttendance)
                    {
                        return;
                    }

                    var allAttendancesString = attendanceString.Split('\n');
                    foreach (var currentAttendanceString in allAttendancesString)
                    {
                        if (currentAttendanceString.IsNullOrEmpty())
                        {
                            continue;
                        }
                        var result = ParsAttendanceLog(deviceInList, currentAttendanceString);
                        if (result != null)
                        {
                            HardwareEventPublisher.Instance.PublishAttendance(result);
                        }
                    }
                }

                if (bufferString.IndexOfEx("Stamp", 1) > 0
                    && bufferString.IndexOfEx("OPERLOG", 1) > 0
                    && bufferString.IndexOfEx("ATTLOG", 1) < 0
                    && bufferString.IndexOfEx("USERPIC", 1) < 0)
                {
                    if (bufferString.IndexOfEx("Expect: 100-continue") > 0
                        && bufferString.IndexOfEx("FP", 1) < 0
                        && bufferString.IndexOfEx("FACE", 1) < 0
                        // Conflic with sample
                        // && bufferString.IndexOfEx("OPERLOG", 1) < 0
                        )
                    {
                        SendDataToDevice("100 Continue", "Continue", deviceSocket);
                        Thread.Sleep(_pushConfig.SleepOnContinueInMillisecond);
                        return;
                    }

                    ProcessNormalDeviceData(bufferString);

                }

                // Visible lights devices
                if (bufferString.IndexOfEx("Stamp", 1) > 0
                    && bufferString.IndexOfEx("BIODATA", 1) > 0)
                {
                    ProcessVisibleLightBioData(bufferString);
                }
                //table=ATTPHOTO
                if (bufferString.IndexOfEx("Stamp", 1) > 0
                    && bufferString.IndexOfEx("ATTPHOTO", 1) > 0)  /* upload attphoto */
                {
                    if (bufferString.IndexOfEx("Expect: 100-continue") > 0
                        && bufferString.IndexOfEx("PIN") < 0)
                    {
                        SendDataToDevice("100 Continue", "Continue", deviceSocket);
                        Thread.Sleep(_pushConfig.SleepOnContinueInMillisecond);
                        return;
                    }
                    ProcessAttPhoto(bufferString, receivedBytes);
                }

                ////table=USERPIC
                if (bufferString.IndexOfEx("Stamp", 1) > 0 && bufferString.IndexOfEx("USERPIC", 1) > 0) // Upload user Info
                {
                    UserPicLog(bufferString);
                }



                ////options Push configuration information
                //if (bufferString.IndexOfEx("table=options", 1) > 0) // Upload options Info
                //{
                //	Options(bufferString);
                //}
                SendDataToDevice(responseCode, responseString, deviceSocket);
            }

        }

        private void ProcessGetRequest(string bufferString, Socket deviceSocket)
        {
            //var bufferString = Encoding.UTF8.GetString(receivedBytes);

            var serialNumber = GetValueByNameInPushHeader(bufferString, "SN");
            var deviceInList = GetDeviceBySerialNumber(serialNumber);
            if (deviceInList == null)
            {
                SendDataToDevice("401 Unauthorized", "Device Unauthorized", deviceSocket);
            }
            else
            {
                _pushDevicesConnectionInfo[deviceInList.Id] = new DeviceConnectionInfo
                {
                    DeviceId = deviceInList.Id,
                    DeviceNumber = deviceInList.DeviceNumber,
                    ConnectionDateTimeUtc = DateTime.Now,
                };
                if (_commandCountDictionary.TryGetValue(serialNumber, out var commandCount) && commandCount > 0)
                {
                    var currentDeviceCommands =
                        _actionToGetCommands(new DeviceNotSentCommandsFilter
                        {
                            Producer = ProducerEnumeration.Zk,
                            SdkVersion = SdkVersionEnumeration.SdkVersion1,
                            DeviceSerialNumbers = new List<string> { serialNumber },
                            Count = _maxCommandCount
                        });
                    if (currentDeviceCommands.IsCollectionNotNullOrEmpty())
                    {
                        var commandBuilder = new StringBuilder();
                        var ids = new List<int>();
                        for (var i = 0; i < _maxCommandCount && i < currentDeviceCommands.Count; i++)
                        {
                            var cmd = currentDeviceCommands[i];
                            var currentCommand = $"C:{cmd.NumericId}:{cmd.CommandContent}\n";
                            if (commandBuilder.Length + currentCommand.Length > MaxBufferSize)
                            {
                                break;
                            }
                            commandBuilder.Append(currentCommand);
                            ids.Add(cmd.NumericId);
                        }
                        HardwareEventPublisher.Instance.PublishCommandSentToDevice(ids);
                        SendDataToDevice("200 OK", $"{commandBuilder}\r\n", deviceSocket);
                    }
                    else
                    {
                        SendDataToDevice("200 OK", "OK\r\n", deviceSocket);
                    }
                }
                else
                {
                    SendDataToDevice("200 OK", "OK\r\n", deviceSocket);
                }
            }
        }

        private void ProcessDeviceCommandResponse(string bufferString, Socket deviceSocket)
        {
            //var bufferString = Encoding.ASCII.GetString(receivedBytes).TrimEnd('\0');
            var serialNumber = GetValueByNameInPushHeader(bufferString, "SN");
            var deviceInList = GetDeviceBySerialNumber(serialNumber);
            var index = bufferString.IndexOfEx("ID=");
            SendDataToDevice("200 OK", "OK\r\n", deviceSocket);
            var contentList1 = bufferString.Substring(index).Split('\n');
            foreach (var content in contentList1)
            {
                if (content.IsNotNullOrEmpty())
                {
                    try
                    {
                        // Check for command response
                        var contentParts = content.Split('&');
                        var commandIdResponse = contentParts[0];
                        if (content.StartsWith("UserCount"))
                        {
                            var commandIdResponseParts = commandIdResponse.Split('=');
                            if (commandIdResponseParts.Length > 1)
                            {
                                var count = commandIdResponseParts[1].ToInt32();
                                if (deviceInList != null)
                                {
                                    HardwareEventPublisher.Instance.PublishUserCountReceived(deviceInList.DeviceNumber, count);
                                }
                            }
                            continue;
                        }
                        if (content.StartsWith("FPCount"))
                        {
                            var commandIdResponseParts = commandIdResponse.Split('=');
                            if (commandIdResponseParts.Length > 1)
                            {
                                var count = commandIdResponseParts[1].ToInt32();
                                if (deviceInList != null)
                                {
                                    HardwareEventPublisher.Instance.PublishFingerCountReceived(deviceInList.DeviceNumber, count);
                                }
                            }
                            continue;
                        }
                        if (content.StartsWith("FaceCount"))
                        {
                            var commandIdResponseParts = commandIdResponse.Split('=');
                            if (commandIdResponseParts.Length > 1)
                            {
                                var count = commandIdResponseParts[1].ToInt32();
                                if (deviceInList != null)
                                {
                                    HardwareEventPublisher.Instance.PublishFaceCountReceived(deviceInList.DeviceNumber, count);
                                }
                            }
                            continue;
                        }
                        if (content.StartsWith("TransactionCount"))
                        {
                            var commandIdResponseParts = commandIdResponse.Split('=');
                            if (commandIdResponseParts.Length > 1)
                            {
                                var count = commandIdResponseParts[1].ToInt32();
                                if (deviceInList != null)
                                {
                                    HardwareEventPublisher.Instance.PublishAccessLogCountReceived(deviceInList.DeviceNumber, count);
                                }
                            }
                            continue;
                        }
                        if (commandIdResponse.IsNotNullOrEmpty())
                        {
                            var commandIdResponseParts = commandIdResponse.Split('=');
                            if (commandIdResponseParts.Length > 1)
                            {
                                var id = commandIdResponseParts[1].ToInt32();
                                if (id > 0)
                                {
                                    var resultOfSend = 0;
                                    if (contentParts.Length > 1)
                                    {
                                        var resultParts = contentParts[1].Split('=');
                                        if (resultParts.Length > 1)
                                        {
                                            int.TryParse(resultParts[1], out resultOfSend);
                                        }
                                    }

                                    if (resultOfSend == 0)
                                    {
                                        var commandResult = new DtoDeviceCommandProcessingResult
                                        {
                                            NumericId = commandIdResponseParts[1].ToInt32(),
                                            CommandResponseTime = DateTime.UtcNow,
                                            CommandResponseResult = content,
                                            Mode = null,
                                        };
                                        HardwareEventPublisher.Instance.PublishCommandResponseReceived(commandResult);
                                    }
                                    else
                                    {
                                        // دستگاه اعلام کرده است که عملیات ناموفق بوده پس باشد در توضیخات کامند ذکر شود
                                        HardwareEventPublisher.Instance.PublishCommandDescriptionReceived(new DtoDeviceCommandProcessingDescription
                                        {
                                            NumericId = commandIdResponseParts[1].ToInt32(),
                                            Description = content,
                                            Mode = null,
                                        });
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogError(exp);
                    }
                }
            }

        }

        public void UserPicLog(string sBuffer)
        {
            var machineSerialNumber = sBuffer.Substring(sBuffer.IndexOfEx("SN=") + 3);
            var serialNumber = machineSerialNumber.Split('&')[0];
            var deviceData = GetDeviceBySerialNumber(serialNumber);
            if (deviceData == null)
            {
                return;
            }
            var userInIndex = sBuffer.IndexOfEx("\r\n\r\n", 1);
            var userInString = sBuffer.Substring(userInIndex + 4);
            if (!userInString.EndsWith("\n"))
            {
                userInString += "\n";
            }
            ProcessUserPic(userInString, deviceData);
        }

        public void ProcessUserPic(string sBuffer, DtoDevice deviceData)
        {
            try
            {
                // ReSharper disable IdentifierTypo
                var usinindex = sBuffer.IndexOfEx("\n", 1);
                var usinstr = "";

                if (usinindex > 0)
                {
                    usinstr = sBuffer.Substring(0, usinindex);
                }
                SaveUserProfileImage(usinstr, deviceData);
                var endop = sBuffer.Substring(usinindex + 1);
                if (endop != "")
                {
                    ProcessUserPic(endop, deviceData);
                }
                // ReSharper restore IdentifierTypo

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }

        }

        public void SaveUserProfileImage(string useLog, DtoDevice deviceData)
        {
            if (useLog.IndexOfEx("PIN") > 0 && useLog.IndexOfEx("FileName") > 0)
            {

                // ReSharper disable IdentifierTypo
                // ReSharper disable InconsistentNaming
                // ReSharper disable UnusedVariable
                var UsInid = useLog.Substring(0, useLog.IndexOfEx("\t"));
                var stillusin1 = useLog.Substring(useLog.IndexOfEx("\t") + 1);
                var usinnum1 = stillusin1.Substring(0, stillusin1.IndexOfEx("\t"));
                var stillusin2 = stillusin1.Substring(stillusin1.IndexOfEx("\t") + 1);
                var usinnum2 = stillusin2.Substring(0, stillusin2.IndexOfEx("\t"));
                var stillusin3 = stillusin2.Substring(stillusin2.IndexOfEx("\t") + 1);
                var stillusin4 = stillusin3.Substring(8);
                var imageData = Convert.FromBase64String(stillusin4);
                var name = usinnum1.Replace("FileName=", "");
                var name1 = name.Substring(0, name.Length - 4);
                HardwareEventPublisher.Instance.PublishUserProfileImageReceived(new DtoUserImage
                {
                    UserIdOnDevice = name1.ToInt64(),
                    PhotoData = imageData
                }, deviceData.DeviceNumber);
                // ReSharper restore InconsistentNaming
                // ReSharper restore IdentifierTypo
                // ReSharper restore UnusedVariable

            }
        }

        private void ProcessAttPhoto(string bufferString, byte[] receivedBytes)
        {
            DtoDevice deviceData = null;
            string[] nameParts = null;
            byte[] imgReceive = null;
            try
            {
                //var bufferString = Encoding.ASCII.GetString(receivedBytes);
                //imgReceive = new byte[receivedBytes.Length];
                var tmpString = bufferString.Split('\n');
                var strImageNumber = "";
                foreach (var str in tmpString)
                {
                    if (str.IndexOfEx("PIN=") >= 0)
                    {
                        strImageNumber = str;
                        break;
                    }
                }
                var machineSerialNumberPure = bufferString.Substring(bufferString.IndexOfEx("SN=") + 3);
                var serialNumber = machineSerialNumberPure.Split('&')[0];
                deviceData = GetDeviceBySerialNumber(serialNumber);
                if (deviceData == null)
                {
                    return;
                }

                var imageRecordIndex = bufferString.IndexOfEx("uploadphoto") + 12;

                imgReceive = ReduceImageSize(receivedBytes, imageRecordIndex);
                //Array.Copy(receivedBytes, imageRecordIndex, imgReceive, 0, receivedBytes.Length - imageRecordIndex);

                nameParts = strImageNumber.Replace("PIN=", "").Split('.')[0]
                    .Split('-');
                if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.ServerPushAttendanceImage))
                {
                    LoggingSystem.LogInfo("ZkServer Attendance Image data", new
                    {
                        ImageName = strImageNumber,
                        NameParts = nameParts,
                        deviceData.DeviceNumber,
                        ImageSize = imgReceive?.Length ?? 0
                    });
                }
                if (imgReceive?.Length > 0)
                {
                    if (nameParts.Length > 1)
                    {
                        // Attendance image
                        var attendanceDatePure = DateTime.ParseExact(nameParts[0], "yyyyMMddHHmmss",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None);
                        var deviceTimeService = new DeviceTimeService();
                        var utcDate = deviceTimeService.DeviceTimeToUtc(attendanceDatePure, deviceData.IanaTimeZoneId);
                        var data = new DtoDeviceAttendanceImage
                        {
                            DeviceId = deviceData.Id,
                            AttendanceDateTime = utcDate,
                            Image = imgReceive
                        };
                        if (long.TryParse(nameParts[1], out var userIdOnDevice))
                        {
                            data.UserIdOnDevice = userIdOnDevice;
                        }

                        HardwareEventPublisher.Instance.PublishAttendanceImage(data);
                        if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.ServerPushAttendanceImage))
                        {
                            LoggingSystem.LogInfo("ZkServer PublishAttendanceImage successfully", new
                            {
                                data.DeviceId,
                                data.UserIdOnDevice,
                                data.AttendanceDateTime,
                                data.Image.Length,

                            });
                        }

                    }
                    else
                    {
                        // Unauthorized attendance image
                        var attendanceDateTimePure = DateTime.ParseExact(nameParts[0], "yyyyMMddHHmmss",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None);
                        var deviceTimeService = new DeviceTimeService();
                        var utcDate = deviceTimeService.DeviceTimeToUtc(attendanceDateTimePure, deviceData.IanaTimeZoneId);
                        var data = new DtoDeviceUnauthorizedAttendanceImage
                        {
                            UserIdInDevice = null,
                            DeviceId = deviceData.Id,
                            AttendanceDateTime = utcDate,
                            Image = imgReceive
                        };
                        HardwareEventPublisher.Instance.PublishUnauthorizedAttendanceImage(data);
                        if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.ServerPushAttendanceImage))
                        {
                            LoggingSystem.LogInfo("ZkServer PublishUnauthorizedAttendanceImage successfully", new
                            {
                                data.DeviceId,
                                data.AttendanceDateTime,
                                data.Image.Length
                            });
                        }

                    }
                }
            }
            catch (Exception exp)
            {
                var deviceNumberString = deviceData != null ? deviceData.DeviceNumber.ToString() : "Unknown";
                var imgReceiveLenString = imgReceive != null ? imgReceive.Length.ToString() : "0";
                var namePartsString = nameParts != null && nameParts.Length > 0 ? string.Join(",", nameParts) : "";
                LoggingSystem.LogError(exp, "Error On parse ZkServer.AttPhoto", $"DeviceNumber: {deviceNumberString} --- imgReceiveLen = {imgReceiveLenString} --- NameParts = {namePartsString}");
            }
        }

        private void ProcessVisibleLightBioData(string bufferString)
        {
            var machineSerialNumberPure = bufferString.Substring(bufferString.IndexOfEx("SN=") + 3);
            var serialNumber = machineSerialNumberPure.Split('&')[0];
            var deviceData = GetDeviceBySerialNumber(serialNumber);
            if (deviceData == null)
            {
                return;
            }

            var bioIndex = bufferString.IndexOfEx("\r\n\r\n", 1);
            var bioDataString = bufferString.Substring(bioIndex + 4);
            var allBioDataSplitted = bioDataString.Split('\n');
            foreach (var bioData in allBioDataSplitted)
            {
                if (string.IsNullOrEmpty(bioData))
                    continue;
                var bioTypeString = bioData.Split('\t')[5].Split('=')[1];
                var bioType = (BioType)Enum.Parse(typeof(BioType), bioTypeString);
                switch (bioType)
                {
                    case BioType.FingerPrint:
                        {
                            if (bioData.IndexOfEx("PIN") >= 0)
                            {
                                var finger = new DtoUserFinger();
                                var template = Replace(bioData, "BIODATA", "");
                                var dic = GetKeyValues(template);

                                finger.UserIdOnDevice = GetValueFromDic(dic, "PIN").ToInt64();
                                finger.FingerIndex = GetValueFromDic(dic, "No").ToInt32();
                                finger.TemplateData = Encoding.UTF8.GetBytes(GetValueFromDic(dic, "TMP"));
                                HardwareEventPublisher.Instance.PublishNewFingerEnrolled(finger, deviceData.DeviceNumber);
                            }
                        }
                        break;
                    case BioType.Face:
                        //case BioType.VisibleLightFace:
                        if (bioData.IndexOfEx("PIN") >= 0)
                        {
                            var template = Replace(bioData, "BIODATA", "");
                            var dic = GetKeyValues(template);

                            var face = new DtoUserFace
                            {
                                UserIdOnDevice = GetValueFromDic(dic, "PIN").ToInt64(),
                                FaceIndex = GetValueFromDic(dic, "No").ToInt32(),
                                TemplateData = Convert.FromBase64String(GetValueFromDic(dic, "TMP"))
                            };
                            face.Length = face.TemplateData.Length;
                            HardwareEventPublisher.Instance.PublishNewFaceEnrolled(face, deviceData.DeviceNumber);
                        }
                        break;
                    case BioType.Palm:
                        if (bioData.IndexOfEx("PIN") >= 0)
                        {
                            var template = Replace(bioData, "BIODATA", "");
                            var dic = GetKeyValues(template);
                            var palm = new DtoUserPalm
                            {
                                UserIdOnDevice = GetValueFromDic(dic, "PIN").ToInt64(),
                                Index = GetValueFromDic(dic, "Index").ToInt32(),
                                TemplateData = Convert.FromBase64String(GetValueFromDic(dic, "TMP"))
                            };
                            palm.Length = palm.TemplateData.Length;
                            HardwareEventPublisher.Instance.PublishNewPalmEnrolled(palm, deviceData.DeviceNumber);
                        }
                        break;

                }

            }
        }

        private void ProcessNormalDeviceData(string bufferString)
        {
            var machineSerialNumberPure = bufferString.Substring(bufferString.IndexOfEx("SN=") + 3);
            var serialNumber = machineSerialNumberPure.Split('&')[0];
            var deviceData = GetDeviceBySerialNumber(serialNumber);
            if (deviceData == null)
            {
                return;
            }

            var bioIndex = bufferString.IndexOfEx("\r\n\r\n", 1);
            var bioDataString = bufferString.Substring(bioIndex + 4);
            var allBioDataSplitted = bioDataString.Split('\n');
            foreach (var record in allBioDataSplitted)
            {
                switch (record.Split(' ')[0])
                {
                    case "USER":
                        {
                            if (record.IndexOfEx("PIN") >= 0)
                            {
                                var userInfo = new DtoUserDeviceRelatedData();
                                var userInfoString = Replace(record, "USER", "");
                                var dic = GetKeyValues(userInfoString);
                                if (long.TryParse(GetValueFromDic(dic, "PIN"), out var employeeNumber)
                                    && short.TryParse(GetValueFromDic(dic, "Pri"), out var privilege))
                                {
                                    var rfCardNumberString = GetValueFromDic(dic, "Card");
                                    userInfo.UserIdOnDevice = employeeNumber;
                                    userInfo.UserName = GetValueFromDic(dic, "Name");
                                    userInfo.Password = GetValueFromDic(dic, "Passwd");
                                    userInfo.Privilege = privilege;
                                    userInfo.RfCardNumbers = rfCardNumberString.IsNotNullOrEmpty()
                                        ? new List<string> { rfCardNumberString } : new List<string>();
                                    userInfo.IsEnable = true;
                                    HardwareEventPublisher.Instance.PublishNewUserEnrolled(userInfo, deviceData.DeviceNumber, DtoUserEnrolledSetting.GetAllSettingInstance());
                                }
                                else
                                {
                                    LoggingSystem.LogWarning($"DeviceNumber : {deviceData.DeviceNumber} ", "Device sent invalid user data");
                                    if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.LogInvalidDeviceData))
                                    {
                                        LoggingSystem.LogInfo("Device sent invalid user data", new
                                        {
                                            deviceData.DeviceNumber,
                                            Message = bufferString,
                                        });
                                    }
                                }
                            }
                        }
                        break;

                    case "FP":
                        {
                            if (record.IndexOfEx("PIN") >= 0)
                            {
                                var finger = new DtoUserFinger();
                                var template = Replace(record, "FP", "");
                                var dic = GetKeyValues(template);
                                if (long.TryParse(GetValueFromDic(dic, "PIN"), out var employeeNumber)
                                    && int.TryParse(GetValueFromDic(dic, "FID"), out var fingerIndex))
                                {
                                    finger.UserIdOnDevice = employeeNumber;
                                    finger.FingerIndex = fingerIndex;
                                    finger.TemplateData = Encoding.UTF8.GetBytes(GetValueFromDic(dic, "TMP"));
                                    HardwareEventPublisher.Instance.PublishNewFingerEnrolled(finger, deviceData.DeviceNumber);
                                }
                                else
                                {
                                    LoggingSystem.LogWarning($"DeviceNumber : {deviceData.DeviceNumber} ", "Device sent invalid finger print");
                                    if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.LogInvalidDeviceData))
                                    {
                                        LoggingSystem.LogInfo("Device sent invalid finger print", new
                                        {
                                            deviceData.DeviceNumber,
                                            Message = bufferString,
                                        });
                                    }
                                }
                            }
                        }
                        break;

                    case "FACE":
                        {
                            if (record.IndexOfEx("PIN") >= 0)
                            {
                                var face = new DtoUserFace();
                                var template = Replace(record, "FACE", "");
                                var dic = GetKeyValues(template);
                                if (long.TryParse(GetValueFromDic(dic, "PIN"), out var employeeNumber))
                                {
                                    face.UserIdOnDevice = employeeNumber;
                                    face.FaceIndex = 50;
                                    face.TemplateData = Encoding.UTF8.GetBytes(GetValueFromDic(dic, "TMP"));
                                    face.Length = face.TemplateData.Length;
                                    HardwareEventPublisher.Instance.PublishNewFaceEnrolled(face, deviceData.DeviceNumber);
                                }
                                else
                                {
                                    LoggingSystem.LogWarning($"DeviceNumber : {deviceData.DeviceNumber} ", "Device sent invalid face");
                                    if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.LogInvalidDeviceData))
                                    {
                                        LoggingSystem.LogInfo("Device sent invalid face", new
                                        {
                                            deviceData.DeviceNumber,
                                            Message = bufferString,
                                        });
                                    }
                                }
                            }
                        }
                        break;

                    case "BIOPHOTO":
                        {
                            if (record.IndexOfEx("PIN") >= 0)
                            {
                                var bioPhotoString = Replace(record, "BIOPHOTO", "");
                                var dic = GetKeyValues(bioPhotoString);
                                var content = GetValueFromDic(dic, "Content");
                                byte[] templateData = null;
                                try
                                {
                                    templateData = Convert.FromBase64String(content);
                                }
                                catch (Exception exp)
                                {
                                    LoggingSystem.LogInfo("ZkServer Oversize image", new { ContentLenght = content?.Length, PIN = GetValueFromDic(dic, "PIN"), deviceData.DeviceNumber });
                                    LoggingSystem.LogError(exp);
                                }
                                if (templateData.IsCollectionNotNullOrEmpty())
                                {
                                    var faceVisibleLight = new DtoUserFace()
                                    {
                                        UserIdOnDevice = GetValueFromDic(dic, "PIN").ToInt64(),
                                        TemplateData = templateData,
                                        Length = 50,
                                    };
                                    // ReSharper disable once PossibleNullReferenceException
                                    faceVisibleLight.Length = faceVisibleLight.TemplateData.Length;
                                    HardwareEventPublisher.Instance.PublishNewFaceEnrolled(faceVisibleLight,
                                        deviceData.DeviceNumber);
                                }
                            }
                        }
                        break;

                    case "OPLOG":
                        {
                            var opLogString = record.Split('\t');
                            try
                            {
                                var operationDateTimePure = Convert.ToDateTime(opLogString[2], new CultureInfo("en-US"));
                                var deviceTimeService = new DeviceTimeService();
                                var operationUtcDateTime = deviceTimeService.DeviceTimeToUtc(operationDateTimePure, deviceData.IanaTimeZoneId);

                                HardwareEventPublisher.Instance.PublishZkOperationLogData(new DtoZkOperationLog
                                {
                                    OperationType = opLogString[0].Substring(6),
                                    Operator = opLogString[1],
                                    OperationTime = operationUtcDateTime,
                                    Object1 = opLogString[3],
                                    Object2 = opLogString[4],
                                    Object3 = opLogString[5],
                                    Object4 = opLogString[6],
                                    User = "0",
                                    DeviceId = deviceData.Id,
                                });
                            }
                            catch (Exception exp)
                            {
                                LoggingSystem.LogError(exp, "ZkServer Error on processing ZK device OPLog data", new
                                {
                                    LogString = opLogString,
                                    deviceData.DeviceNumber
                                });
                                //throw;
                            }
                        }
                        break;
                }
            }

        }

        private void SendDeviceConfig(DtoDevice deviceInfo, Socket deviceSocket)
        {
            if (deviceInfo != null)
            {
                var time = 0;
                try
                {
                    var deviceTimeService = new DeviceTimeService();
                    var timezone = deviceTimeService.GetUtcOffsetString(deviceInfo.IanaTimeZoneId);
                    string[] splittedString;
                    if ('-' == timezone[0])
                    {
                        timezone = timezone.Substring(1);
                        splittedString = timezone.Split(':');
                        switch (splittedString.Length)
                        {
                            case 1:
                                time = -1 * (Convert.ToInt32(splittedString[0]) * 60);
                                break;
                            case 2:
                                time = -1 * (Convert.ToInt32(splittedString[0]) * 60 + Convert.ToInt32(splittedString[1]));
                                break;

                        }
                    }
                    else
                    {
                        splittedString = timezone.Split(':');
                        switch (splittedString.Length)
                        {
                            case 1:
                                time = Convert.ToInt32(splittedString[0]) * 60;
                                break;
                            case 2:
                                time = Convert.ToInt32(splittedString[0]) * 60 + Convert.ToInt32(splittedString[1]);
                                break;

                        }
                    }
                }
                catch
                {
                    time = 0;
                }
                var sb = new StringBuilder();
                sb.Append($"GET OPTION FROM:{deviceInfo.SerialNumber}\n");
                sb.Append($"Stamp={_pushConfig.Stamp}\n");
                sb.Append($"OpStamp={_pushConfig.OpStamp}\n");
                sb.Append($"PhotoStamp={_pushConfig.PhotoStamp}\n");
                sb.Append("TransFlag=TransData AttLog\tOpLog\tAttPhoto\tEnrollUser\tChgUser\tEnrollFP\tChgFP\tFPImag\tFACE\tUserPic\tWORKCODE\tBioPhoto\n");
                sb.Append($"ErrorDelay={_pushConfig.ErrorDelay}\n");
                sb.Append($"Delay={_pushConfig.Delay}\n");
                sb.Append($"TimeZone={time}\n");
                sb.Append($"TransTimes={_pushConfig.TransTimes.ToNotNullString()}\n");
                sb.Append($"TransInterval={_pushConfig.TransInterval}\n");
                sb.Append($"SyncTime={_pushConfig.SyncTime}\n");
                sb.Append($"Realtime={_pushConfig.Realtime}\n");
                sb.Append($"ServerVer={ServerVersion} {DateTime.Now.ToShortDateString()}\n");
                sb.Append($"PushProtVer={PushProtocolVersion}\n");
                sb.Append($"PushOptionsFlag={PushOptionsFlag}\n");
                sb.Append($"ATTLOGStamp={_pushConfig.AttendanceLogStamp}\n");
                sb.Append($"OPERLOGStamp={_pushConfig.OperationLogStamp}\n");
                sb.Append($"ATTPHOTOStamp={_pushConfig.AttendancePhotoStamp}\n");
                sb.Append($"ServerName=Logtime Server\n");
                sb.Append($"MultiBioDataSupport={_pushConfig.MultiBioDataSupport}\n");
                sb.Append($"MultiBioPhotoSupport={_pushConfig.MultiBioPhotoSupport}\n");
                var dataToSend = sb.ToString();

                if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.ServerPushConfig))
                {
                    LoggingSystem.LogInfo("ZkServer push config", new { deviceInfo.DeviceNumber, ConfigData = dataToSend });
                }
                SendDataToDevice("200 OK", dataToSend, deviceSocket);
            }
            else
            {
                SendDataToDevice("401 Unauthorized", "Device Unauthorized", deviceSocket);
            }
        }

        private static void SendDataToDevice(string statusCode, string dataToSend, Socket deviceSocket)
        {
            var contentBytes = Encoding.UTF8.GetBytes(dataToSend);
            var sb = new StringBuilder();
            sb.Append($"HTTP/1.1 {statusCode}\r\n");
            sb.Append("Content-Type: text/plain\r\n");
            sb.Append("Accept-Ranges: bytes\r\n");
            sb.Append($"Date: {DateTime.Now.ToUniversalTime():r}\r\n");
            sb.Append($"Content-Length: {contentBytes.Length}\r\n\r\n");
            var headerBytes = Encoding.UTF8.GetBytes(sb.ToString());

            try
            {
                if (deviceSocket.Connected)
                {
                    if (deviceSocket.Send(headerBytes, headerBytes.Length, 0) == -1
                        || deviceSocket.Send(contentBytes, contentBytes.Length, 0) == -1
                    )
                    {
                        LoggingSystem.LogInfo("ZkServer Error on SendDataToDevice", "Socket Error: Cannot Send Packet");
                    }
                }
                else
                {
                    LoggingSystem.LogInfo("ZkServer Error on SendDataToDevice", "Link Failed...");
                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp);
            }
        }

        private static DtoAttendance ParsAttendanceLog(DtoDevice deviceInfo, string attendanceString)
        {
            var attendanceStringSplitted = attendanceString.Split('\t');
            var status = Convert.ToInt32(attendanceStringSplitted[2]);

            var attStatus = (short)(status & 0x7F);
            try
            {
                var attendanceDatePure = Convert.ToDateTime(attendanceStringSplitted[1], new CultureInfo("en-US"));
                DeviceTimeService deviceTimeService = new DeviceTimeService();
                var attendanceDateUtc = deviceTimeService.DeviceTimeToUtc(attendanceDatePure, deviceInfo.IanaTimeZoneId);
                return new DtoAttendance
                {
                    UserIdOnDevice = long.Parse(attendanceStringSplitted[0]),
                    AttendanceDateTime = attendanceDateUtc,
                    VerificationStyle = (int)ZkUtils.GetVerificationStyle(Convert.ToInt16(attendanceStringSplitted[3])),
                    DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                    AttendanceSource = AttendanceSourceEnumeration.Device,
                    DeviceId = deviceInfo.Id,
                    CameraId = null,
                    StatusCode = attStatus,
                    IsSentToGuardian = false,
                    // به صورت پیش فرض برای دستگاه های کنترل تردد آنلاین در صورتی که ذکر شده بود برای دستگاه صحت
                    // تردد بر اساس رویداد گذر باید چک شود، می بایست تردد به صورت پچیش فرض غیر مجاز فرض گردد
                    RfCardNumber = null,
                };
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error On ZkServer.ParsAttendanceLog", new
                {
                    deviceInfo.DeviceNumber,
                    AttendanceString = attendanceString
                });
            }

            return null;
        }

        private DtoDevice GetDeviceBySerialNumber(string deviceSerialNumber)
        {
            DtoDevice result;
            lock (_pushDevices)
            {
                result = _pushDevices.FirstOrDefault(row => row.SerialNumber == deviceSerialNumber);
            }
            if (result == null)
            {
                if (AppConfigs.LogLevelZk.HasFlag(LogLevelZkEnumeration.LogInvalidDevice))
                {
                    LoggingSystem.LogInfo("ZkServer INVALID DEVICE", new { SerialNumber = deviceSerialNumber });
                }
            }
            return result;
        }


        #region Tools

        private byte[] ReduceImageSize(byte[] receivedBytes, int imageIndex)
        {

            try
            {
                using (var image = Image.FromStream(new MemoryStream(receivedBytes, imageIndex, receivedBytes.Length - imageIndex)))
                {
                    using (var ms = new MemoryStream())
                    {
                        image.Save(ms, ImageFormat.Jpeg);
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        //private static void LogInfo(string title, dynamic dataToLog)
        //{
        //    if (AppConfigs.LogLevel.HasFlag(LogLevelEnumeration.LogZkPushData))
        //    {
        //        LoggingSystem.LogInfo(title, new
        //        {
        //            DataToLog = dataToLog,
        //        });
        //    }

        //}

        private static string GetValueByNameInPushHeader(string buffer, string name)
        {
            var splitStr = buffer.Split('&', '?', ' ');
            if (splitStr.Length <= 0)
            {
                return null;
            }

            foreach (var tmpString in splitStr)
            {
                if (tmpString.IndexOfEx(name + "=") >= 0)
                {
                    return tmpString.Substring(tmpString.IndexOfEx(name + "=") + name.Length + 1);
                }
            }
            return null;
        }

        private static string Replace(string str, string oldStr, string newStr, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            var idx = str.IndexOf(oldStr, stringComparison);
            if (idx <= -1)
                return str;

            var sb = new StringBuilder();
            sb.Append(str.Substring(0, idx));
            sb.Append(newStr);
            sb.Append(str.Substring(idx + oldStr.Length));

            return sb.ToString();
        }

        private static Dictionary<string, string> GetKeyValues(string str, char cSplit = '\t', char splitKeyValue = '=', bool keyToLower = true)
        {
            var dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(str))
                return dic;

            var arr = str.Split(cSplit);
            foreach (var kv in arr)
            {
                var idx = kv.IndexOf(splitKeyValue);
                if (idx <= 0)
                    continue;

                var key = kv.Substring(0, idx).Trim();
                if (keyToLower)
                    key = key.ToLower();

                if (string.IsNullOrEmpty(key) || dic.ContainsKey(key))
                    continue;

                dic.Add(key, kv.Substring(idx + 1));
            }

            return dic;
        }

        private static string GetValueFromDic(Dictionary<string, string> dic, string key, string defaultVal = "", bool keyToLower = true)
        {
            if (string.IsNullOrEmpty(key))
                return defaultVal;

            if (keyToLower)
                key = key.Trim().ToLower();

            return dic.TryGetValue(key, out var value) ? value : defaultVal;
        }

        #endregion


        #region Type

        public class DeviceConnectionInfo
        {
            public Guid DeviceId { get; set; }

            public int DeviceNumber { get; set; }

            public DateTime ConnectionDateTimeUtc { get; set; }
        }


        #endregion

        #endregion


        #region Implementation of IDisposable

        private bool _disposed;

        /// <summary>
        ///   Releases all resources used by the WarrantManagement.DataExtract.Dal.ReportDataBase
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///   Releases all resources used by the WarrantManagement.DataExtract.Dal.ReportDataBase
        /// </summary>
        /// <param name="disposing"> A boolean value indicating whether or not to dispose managed resources </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                // Managed teardown only on explicit Dispose — never on the finalizer thread.
                StopPushListening();
            }

            _disposed = true;
        }

        #endregion


    }

    // ReSharper disable UnusedMember.Global
    internal enum BioType
    {
        Comm = 0,
        FingerPrint = 1,
        Face = 2,
        VocalPrint = 3,
        Iris = 4,
        Retina = 5,
        PalmPrint = 6,
        FingerVein = 7,
        Palm = 8,
        VisibleLightFace = 9
    }
    // ReSharper restore UnusedMember.Global


}

