using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EosClocks;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.ElmOSanat.Concepts;

namespace GuardianCommunication.Hardware.ElmOSanat
{
    public class ElmoSanatOnDemandAdapter : IDisposable
    {
        private bool _isDeviceConnected;
        private bool _continueGetRecords;
        private Connection _connection;
        private Clock _clock;
        public DtoCommunicationDeviceData DeviceInfo { get; set; }

        public ElmoSanatOnDemandAdapter(DtoCommunicationDeviceData deviceInfo)
        {
            DeviceInfo = deviceInfo;
        }

        public void Connect()
        {
            if (_isDeviceConnected) return;
            switch (DeviceInfo.ConnectionTypeEnum)
            {
                case ConnectionTypeEnumeration.Rs485:
                case ConnectionTypeEnumeration.Rs232:
                    {
                        if (string.IsNullOrEmpty(DeviceInfo.Ip))
                            throw new OperationCannotBeDoneException

                                (OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorIpIsNotValid);
                        if (!DeviceInfo.TcpPort.HasValue || DeviceInfo.TcpPort.Value <= 0)
                            throw new OperationCannotBeDoneException
                                (OperationResultEnumeration.CommunicationStatusSupremaSdk1ErrorTcpPortIsNotValid);
                        const int waitBeforeRead = 0;
                        const int writeTimeout = 500;
                        const int readTimeout = 1000;
                        const int addressNup = 1;
                        var protocolType = ProtocolType.RS232;
                        switch (DeviceInfo.ConnectionTypeEnum)
                        {
                            case ConnectionTypeEnumeration.Rs485:
                                protocolType = ProtocolType.RS485;
                                break;
                            default:
                                protocolType = ProtocolType.RS232;
                                break;
                        }
                        _connection = ConnectionFactory.CreateTCPIPConnection(DeviceInfo.Ip, DeviceInfo.TcpPort.Value, waitBeforeRead, readTimeout, writeTimeout);
                        _clock = new Clock(_connection, protocolType, addressNup, ProtocolType.Suprema);
                        // Bounded, cross-platform connect (replaces Delegate.BeginInvoke): run the
                        // blocking TestConnection on a task and commit the result only if it finished
                        // in time, so a late completion can't flip _isDeviceConnected after failure.
                        var connectTask = Task.Run(() => _clock.TestConnection());
                        var connected = false;
                        try
                        {
                            if (connectTask.Wait(DeviceInfo.ConnectTimeout * 1000))
                            {
                                connected = connectTask.Result;
                            }
                            else
                            {
                                // Timed out: observe any later fault so it isn't an unobserved task exception.
                                connectTask.ContinueWith(t => { _ = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
                            }
                        }
                        catch (AggregateException)
                        {
                            connected = false;
                        }
                        _isDeviceConnected = connected;
                        if (!_isDeviceConnected)
                        {
                            throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusCannotConnect);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void SetDateTime()
        {
            if (_clock == null || !_isDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            if (AppConfigs.LogLevelElmoSanat.HasFlag(LogLevelElmoSanatEnumeration.SetDateTime))
            {
                LoggingSystem.LogInfo("ElmoSanat OnDemand SetDateTime", new { DeviceInfo });
            }
            _clock.SetDateTime(DateTime.Now);
        }

        public DateTime GetDateTime()
        {
            if (_clock == null || !_isDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            var result = _clock.GetDateTime();
            if (AppConfigs.LogLevelElmoSanat.HasFlag(LogLevelElmoSanatEnumeration.GetDateTime))
            {
                LoggingSystem.LogInfo("ElmoSanat OnDemand GetDateTime", new { DeviceInfo, Date = result });
            }
            return result;
        }

        public List<DtoAttendance> GetData()
        {
            if (_clock == null || !_isDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);

            if (AppConfigs.LogLevelElmoSanat.HasFlag(LogLevelElmoSanatEnumeration.GetData))
            {
                LoggingSystem.LogInfo("ElmoSanat OnDemand GetData is calling", new { DeviceInfo });
            }

            if (DeviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
            {
                throw new OperationCannotBeDoneException(OperationResultEnumeration
                    .CommunicationStatusDeviceAttendanceCollectionIsNotActive);
            }
            const int i = 0;
            var attRecords = new List<DtoAttendance>();
            _continueGetRecords = true;
            while (_continueGetRecords)
            {
                if (_clock != null && _clock.IsEmpty())
                {
                    if (AppConfigs.LogLevelElmoSanat.HasFlag(LogLevelElmoSanatEnumeration.GetData))
                    {
                        LoggingSystem.LogInfo("ElmoSanat OnDemand clock is empty", new { DeviceInfo });
                    }
                    break;
                }
                try
                {
                    if (_clock != null)
                    {
                        var record = (ClockRecord)_clock.GetRecord();
                        var row = new DtoAttendance
                        {
                            Id = i + 1,
                            EmployeeNumber = (uint)record.ID,
                            AttendanceDateTime = record.DateTime,
                            AttendanceSource = AttendanceSourceEnumeration.Device,
                            DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.OnDemand,
                            DeviceNumber = DeviceInfo.DeviceNumber,
                            CameraId = null,
                            IsInvalid = false,
                            StatusCode = 0,
                            VerificationStyle = (int)ElmOSanatVerificationStyleEnumeration.FingerPrint,
                            RfCardNumber = null,
                        };
                        attRecords.Add(row);
                    }
                    _clock.NextRecord();
                }
                catch (Exception exp)
                {
                    if (exp is InvalidRecordException || exp is InvalidDataInRecordException)
                    {
                        LoggingSystem.LogError(exp, "ElmoOSanat Bad Record", exp.Data["RecordRawData"]);
                    }
                    else
                    {
                        LoggingSystem.LogError(exp);
                    }
                }
            }

            if (AppConfigs.LogLevelElmoSanat.HasFlag(LogLevelElmoSanatEnumeration.GetData))
            {
                LoggingSystem.LogInfo("ElmoSanat OnDemand GetData result", new { DeviceInfo, Records = attRecords });
            }

            return attRecords;
        }

        public int GetRecordCount()
        {
            if (_clock == null || !_isDeviceConnected)
                throw new OperationCannotBeDoneException(OperationResultEnumeration.CommunicationStatusConnectTheDeviceFirst);
            if (AppConfigs.LogLevelElmoSanat.HasFlag(LogLevelElmoSanatEnumeration.RecordCount))
            {
                LoggingSystem.LogInfo("ElmoSanat OnDemand GetRecordCount is calling", new { DeviceInfo });
            }
            var count = 0;
            if (_clock.IsEmpty())
                return count;
            var clockModel = _clock.GetModel();
            var isStSharp = clockModel.IndexOf("ST-SH", StringComparison.Ordinal) >= 0;
            var readPointer = _clock.GetReadPointer();
            var writePointer = _clock.GetWritePointer();
            if (readPointer < writePointer)
                count = writePointer - readPointer;
            else
            {
                if (isStSharp)
                    count = 253579 - readPointer + writePointer;
                else
                    count = 19500 - readPointer + writePointer;
            }
            if (AppConfigs.LogLevelElmoSanat.HasFlag(LogLevelElmoSanatEnumeration.RecordCount))
            {
                LoggingSystem.LogInfo("ElmoSanat OnDemand GetRecordCount result", new { DeviceInfo, Result = count });
            }
            return count;
        }

        public bool TestConnection()
        {
            Connect();
            return _isDeviceConnected;
        }

        public void Disconnect()
        {
            if (_clock == null) return;
            _isDeviceConnected = false;
            _clock.Disconnect();
            _connection.Disconnect();
            _clock.Dispose();
            _clock = null;
        }


        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                // Managed teardown only on explicit Dispose — never on the finalizer thread.
                if (_isDeviceConnected)
                {
                    Disconnect();
                }
            }
            _disposed = true;
        }

        #endregion

    }
}