using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.Shared;

namespace GuardianCommunication.Hardware.MetalDetectorGate.Padis
{
    [SuppressMessage("ReSharper", "CommentTypo")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("ReSharper", "InvertIf")]
    [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
    public class PadisMetalDetectorGateServer : IDisposable
    {
        private PadisMetalDetectorGateConfig _config;
        private volatile bool _listenFlag = true;
        private Thread _threadWatch;
        private Thread _threadSendData;
        private Socket _socketWatch;
        private ListenState _listenState;
        // Single authoritative collection keyed by remote endpoint (ip:port). Replaces the three
        // index-aligned parallel lists (_socConnections/_dictThread/_clients) that caused the races.
        private readonly ConcurrentDictionary<string, ClientConnection> _connections =
            new ConcurrentDictionary<string, ClientConnection>();
        private const bool IsInSysSet = false;

        #region Singleton

        public static PadisMetalDetectorGateServer Instance { get; }

        private PadisMetalDetectorGateServer()
        {
        }

        static PadisMetalDetectorGateServer()
        {
            Instance = new PadisMetalDetectorGateServer();
        }

        #endregion


        public void StartPadisMetalDetectorGateServer(PadisMetalDetectorGateConfig config, List<DtoMetalDetectorGate> deviceInfos)
        {
            _config = config;
            LoggingSystem.LogInfo("PadisMetalDetectorGateServer started", new
            {
                Config = _config
            });
            var thread = new Thread(() => DoStartServerProcess(deviceInfos));
            thread.Start();
        }

        private void DoStartServerProcess(List<DtoMetalDetectorGate> deviceInfos)
        {
            SetDeviceOnPushModeList(deviceInfos);
            var thread = new Thread(StartPushListening);
            thread.Start();
        }

        public void StopServer()
        {
            Dispose(true);
        }

        private readonly List<DtoMetalDetectorGate> _pushDevices = new List<DtoMetalDetectorGate>();

        public void SetDeviceOnPushModeList(List<DtoMetalDetectorGate> deviceInfos)
        {
            if (deviceInfos.IsCollectionNullOrEmpty())
            {
                return;
            }

            var pushDeviceInfos = deviceInfos.Where
                (row => row.DeviceType == MetalDetectorGateTypeEnumeration.Pd318
                        && row.ConnectionMode == MetalDetectorGateConnectionModeEnumeration.Push).ToList();
            if (pushDeviceInfos.IsCollectionNullOrEmpty())
            {
                return;
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
                        , "Error on SetDeviceOnPushModeList");
                }
            }

        }

        public void StartPushListening()
        {
            try
            {
                _listenFlag = true;
                _socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var localEp = _config.Ip.IsNullOrEmpty() ? new IPEndPoint(IPAddress.Any, _config.Port) : new IPEndPoint(IPAddress.Parse(_config.Ip), _config.Port);
                _socketWatch.Bind(localEp);
                _socketWatch.Listen(20);
                _threadWatch = new Thread(WatchConnecting)
                {
                    IsBackground = true
                };
                _threadWatch.Start();
                _threadSendData = new Thread(PollingDevice)
                {
                    IsBackground = true
                };
                _threadSendData.Start();


            }
            catch (Exception exp)
            {
                _listenState = ListenState.UnListen;
                LoggingSystem.LogError(exp, $"Error on listening to the port {_config.Port}");
            }
        }

        public void StopPushListening()
        {
            // Cooperative shutdown: clear the flag and close the sockets so the watch/listen/poll loops
            // unblock and exit on their own, then join. Closing a connection's socket ends its listen thread.
            _listenFlag = false;
            _socketWatch?.Close();
            var connections = _connections.Values.ToList();
            foreach (var connection in connections)
            {
                RemoveConnection(connection);
            }
            foreach (var connection in connections)
            {
                connection.ListenThread?.Join(2000);
            }
            _threadWatch?.Join(2000);
            _threadSendData?.Join(2000);
            _listenState = ListenState.UnListen;
        }

        #region Private methods


        private void PollingDevice()
        {
            while (_listenFlag)
            {
                try
                {


                    foreach (var connection in _connections.Values)
                    {
                        var st3 = connection.Info;
                        var dat = new byte[] { 0, 4, 1, 0x7f };
                        if (st3.doorId == 0)
                        {
                            dat[0] = 0x80;
                            dat[2] = 0;
                            SendData(st3.ip, st3.port, dat);
                        }
                        else
                        {
                            dat[0] = (byte)(st3.doorId & 0xff);
                            dat[2] = 1;
                            if (IsInSysSet)
                            {
                                Thread.Sleep(0x3e8);
                            }
                            SendData(st3.ip, st3.port, dat);
                        }
                    }

                    Thread.Sleep(100);
                }
                catch (Exception)
                {
                    //Ignore
                }
            }
        }

        private bool SendData(string ip, int port, byte[] dat)
        {
            // Called only from the single PollingDevice thread, so no extra lock is needed.
            var connection = _connections.Values.FirstOrDefault(c => c.Ip == ip && c.Port == port);
            if (connection == null)
            {
                return false;
            }
            try
            {
                connection.Socket.Send(dat);
            }
            catch
            {
                // Send failed: drop the connection (closing its socket ends its listen thread).
                RemoveConnection(connection);
            }
            return false;
        }

        private void WatchConnecting()
        {
            while (_listenFlag)
            {
                try
                {
                    var socket = _socketWatch.Accept();
                    var endpoint = socket.RemoteEndPoint.ToString();
                    var ip = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
                    var port = ((IPEndPoint)socket.RemoteEndPoint).Port;

                    // Exact same endpoint already connected: ignore and close the duplicate socket.
                    if (_connections.ContainsKey(endpoint))
                    {
                        CloseSocket(socket);
                        continue;
                    }

                    // Same device (IP) reconnecting on a different port: drop the old connection first.
                    // Closing its socket makes its listen thread exit on its own (no Thread.Abort).
                    foreach (var existing in _connections.Values.Where(c => c.Ip == ip).ToList())
                    {
                        RemoveConnection(existing);
                    }

                    var connection = new ClientConnection
                    {
                        Socket = socket,
                        Endpoint = endpoint,
                        Info = new ClientInfoSt
                        {
                            doorId = 0,
                            ip = ip,
                            port = port,
                            doorName = "",
                            doorDesc = "",
                            offJudgecnt = 0,
                            offlineFlag = 0,
                            sensitivity = 0,
                            product = ProductType.None
                        }
                    };
                    if (_connections.TryAdd(endpoint, connection))
                    {
                        connection.ListenThread = new Thread(() => ListenSocket(connection)) { IsBackground = true };
                        connection.ListenThread.Start();
                    }
                    else
                    {
                        CloseSocket(socket);
                    }
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on PadisMetalDetectorGateServer.WatchConnecting");
                }
            }
        }

        private void ListenSocket(ClientConnection connection)
        {
            var socket = connection.Socket;
            var flag = true;
            var str = connection.Ip;
            var port = connection.Port;
            var buffer = new byte[0x400];
            while (flag)
            {
                try
                {
                    if ((!socket.Poll(0x7530, SelectMode.SelectRead) || socket.Available != 0) && socket.Connected)
                    {
                        try
                        {
                            var length = socket.Receive(buffer);
                            ProcessSocketData(connection, buffer, length, str, port);
                            continue;
                        }
                        catch (Exception)
                        {
                            flag = false;
                            RemoveConnection(connection);
                        }
                    }
                    else
                    {
                        flag = false;
                        RemoveConnection(connection);
                    }
                    break;
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on PadisMetalDetectorGateServer.ServerRecMsg");
                }
            }
        }

        private void ProcessSocketData(ClientConnection connection, byte[] data, int length, string ip, int port)
        {
            try
            {
                if (length >= 3)
                {
                    var index = data[1] - 1;
                    if (index < 0 || index >= length)
                    {
                        // Malformed/truncated packet — ignore rather than index out of bounds.
                        return;
                    }
                    uint num4 = data[index];
                    var command = (Command)data[2];
                    if (command == Command.M0)
                    {
                        // Update this connection's device info in place (fixes the previous bug where
                        // sensitivity was written to a throwaway local and lost).
                        if (num4 == 0x7f && connection.Info.doorId == 0)
                        {
                            connection.Info.doorId = data[0] & 0xff;
                            connection.Info.product = length <= 4 ? ProductType.None : PadisMetalDetectorGatesHelper.GetProductTypeByInt(data[3]);
                            if (length > 6)
                            {
                                connection.Info.sensitivity = (data[4] << 7) | data[5];
                            }
                        }
                    }
                    else
                    {
                        switch (command)
                        {
                            case Command.M10:
                                if (num4 == 0x7f)
                                {
                                }
                                break;

                            case Command.M11:
                                if (num4 == 0x7f)
                                {
                                    ProcessPassData(data, length, ip);
                                }
                                break;

                            case Command.M12:
                            case Command.M13:
                                if (num4 == 0x7f)
                                {
                                    //ProcessPassData(data);
                                }
                                break;

                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggingSystem.LogError(e, "Error on PadisMetalDetectorGateServer.ProcessSocketData");
                throw;
            }

        }

        private static void ProcessPassData(byte[] data, int length, string deviceIp)
        {
            try
            {
                if (length < 12)
                {
                    // Not enough bytes for a full pass-data packet; ignore.
                    return;
                }

                var alarmStatus = new byte[16];
                alarmStatus[0] = (byte)(data[9] & 1);
                alarmStatus[1] = (byte)((data[9] >> 1) & 1);
                alarmStatus[2] = (byte)((data[9] >> 2) & 1);
                alarmStatus[3] = (byte)((data[9] >> 3) & 1);
                alarmStatus[4] = (byte)((data[9] >> 4) & 1);
                alarmStatus[5] = (byte)((data[9] >> 5) & 1);
                alarmStatus[6] = (byte)(data[10] & 1);
                alarmStatus[7] = (byte)((data[10] >> 1) & 1);
                alarmStatus[8] = (byte)((data[10] >> 2) & 1);
                alarmStatus[9] = (byte)((data[10] >> 3) & 1);
                alarmStatus[10] = (byte)((data[10] >> 4) & 1);
                alarmStatus[11] = (byte)((data[10] >> 5) & 1);
                alarmStatus[12] = (byte)(data[11] & 1);
                alarmStatus[13] = (byte)((data[11] >> 1) & 1);
                alarmStatus[14] = (byte)((data[11] >> 2) & 1);
                alarmStatus[15] = (byte)((data[11] >> 3) & 1);
                var deviceId = data[0];

                var num2 = (data[3] * 0x80) + data[4];
                var num3 = (data[5] * 0x80) + data[6];
                var alarm = (data[7] * 0x80) + data[8];
                var passed = num2 + num3;
                var dataForEvent = new DtoMetalDetectorPersonPassedData()
                {
                    Date = DateTime.Now,
                    TotalAlarm = alarm,
                    TotalPassed = passed,
                    DeviceId = deviceIp,
                    ZoneData = alarmStatus,
                };
                if (AppConfigs.LogLevelMetalDetector.HasFlag(LogLevelMetalDetectorEnumeration.LogGateData))
                {
                    LoggingSystem.LogInfo("Metal Detector gate data", dataForEvent);
                }
                HardwareEventPublisher.Instance.PublishMetalDetectorPersonPassed(dataForEvent);
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on PadisMetalDetectorGateServer.DataReceived");
            }

        }

        private void CloseSocket(Socket socket)
        {
            try
            {

                socket.Close();
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "error close socket");
            }
        }


        // Removes a connection from the collection and closes its socket. Closing the socket causes the
        // connection's ListenSocket thread to exit on its own (this replaces the old Thread.Abort calls).
        // Idempotent: safe to call more than once for the same connection.
        private void RemoveConnection(ClientConnection connection)
        {
            if (connection == null)
            {
                return;
            }
            _connections.TryRemove(connection.Endpoint, out _);
            CloseSocket(connection.Socket);
        }

        // Bundles everything for one live device connection, replacing the three index-aligned lists.
        private sealed class ClientConnection
        {
            public Socket Socket;
            public Thread ListenThread;
            public string Endpoint;
            public ClientInfoSt Info;
            public string Ip => Info.ip;
            public int Port => Info.port;
        }


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


}
