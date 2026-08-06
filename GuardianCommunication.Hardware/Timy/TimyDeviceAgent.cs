using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.Shared;
using Newtonsoft.Json.Linq;
using SuperWebSocket;

namespace GuardianCommunication.Hardware.Timy
{
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class TimyDeviceAgent : IDisposable
    {
        private readonly WebSocketSession _socketSession;
        private readonly TimyPushConfig _pushConfig;

        // فقط یک دستور هم‌زمان می‌تواند در ارسال شود.
        // وقتی دستوری فرستاده می‌شود، یک TaskCompletionSource ساخته می‌شود؛
        // وقتی پاسخ دستگاه می‌رسد (یا تایم‌اوت می‌شود)، همان TCS کامل می‌شود.
        private readonly object _stateLock = new object();
        private TaskCompletionSource<CommandAckResult> _pendingAck;
        private readonly object _sendLock = new object();
        //private DtoDeviceUnsentCommand _pendingCommand = null;

        public DtoCommunicationDeviceData DeviceInfo { get; internal set; }
        /// <summary>
        /// آیا این دستگاه الان آزاد است (منتظر جواب دستور قبلی نیست)؟
        /// TimyServer قبل از fetch از دیتابیس این را چک می‌کند تا فقط دستور دستگاه‌های آزاد را بخواند.
        /// </summary>
        public bool IsIdle
        {
            get { lock (_stateLock) return _pendingAck == null; }
        }



        public TimyDeviceAgent(
            WebSocketSession socketSession
            , DtoCommunicationDeviceData deviceInfo
            , TimyPushConfig pushConfig)
        {
            _socketSession = socketSession;
            _pushConfig = pushConfig;
            DeviceInfo = deviceInfo;
        }

        // ───────────────────────────────────────────────
        //  پردازش پیام‌های ورودی از همین دستگاه
        // ───────────────────────────────────────────────

        public void HandleMessage(JObject jsonMessage)
        {
            var cmd = jsonMessage.Value<string>("cmd");
            var ret = jsonMessage.Value<string>("ret");

            if (cmd.IsNotNullOrEmpty())
            {
                HandleRealtimeCommand(jsonMessage, cmd);
            }
            else if (ret.IsNotNullOrEmpty())
            {
                HandleDeviceResponse(jsonMessage, ret);
            }
        }

        private void HandleRealtimeCommand(JObject jsonMessage, string cmd)
        {
            switch (cmd)
            {
                case "sendlog":
                    ProcessAttendanceLog(jsonMessage);
                    break;

                case "senduser":
                    ProcessUserData(jsonMessage, true);
                    break;

                    // "reg" برای reconnect مدیریت می‌شود، نه اینجا چون TimyServer
                    // باید قبل از وجود این Session پاسخ register را بدهد.
            }
        }

        // وقتی پاسخ دستگاه (ret) از سوکت می‌رسد، فقط TCS را کامل می‌کند؛
        // ادامه‌ی کار (await در TrySendCommandAsync) خودش بیدار می‌شود.
        private void HandleDeviceResponse(JObject jsonMessage, string ret)
        {
            TaskCompletionSource<CommandAckResult> tcs;
            lock (_stateLock)
            {
                tcs = _pendingAck;
            }

            if (tcs == null) return; // پاسخی که منتظرش نبودیم (مثلاً بعد از timeout دیر رسیده)

            var isSuccessful = jsonMessage.Value<bool?>("result") ?? false;
            var reason = isSuccessful ? null : jsonMessage.Value<string>("reason");
            var resultForSet = new CommandAckResult
            {
                IsSuccessful = isSuccessful,
                ResponseTime = DateTime.Now
            };
            var continueWait = false;
            switch (ret)
            {
                case "getuserinfo":
                    if (isSuccessful)
                    {
                        ProcessUserData(jsonMessage, false);
                    }
                    else
                    {
                        resultForSet.ErrorMessage = reason;
                        resultForSet.ResponseTime = DateTime.Now;
                    }
                    break;
                case "getnewlog":
                    if (isSuccessful)
                    {
                        continueWait = ProcessReadAttendanceLog(jsonMessage, "getnewlog");
                    }
                    else
                    {
                        resultForSet.ErrorMessage = reason;
                        resultForSet.ResponseTime = DateTime.Now;
                    }
                    break;
                case "getalllog":
                    if (isSuccessful)
                    {
                        continueWait = ProcessReadAttendanceLog(jsonMessage, "getnewlog");
                    }
                    else
                    {
                        resultForSet.ErrorMessage = reason;
                        resultForSet.ResponseTime = DateTime.Now;
                    }
                    break;
                default:
                    if (!isSuccessful)
                    {
                        resultForSet.ErrorMessage = reason;
                        resultForSet.ResponseTime = DateTime.Now;
                    }
                    break;
            }


            if (!continueWait)
            {
                tcs.TrySetResult(resultForSet);
            }
        }


        private void ProcessAttendanceLog(JObject jsonMessage)
        {
            try
            {
                if (DeviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
                {
                    return;
                }

                var logIndex = jsonMessage.Value<string>("logindex");
                var attRecords = jsonMessage["record"];

                foreach (var ss in attRecords)
                {
                    var userId = ss.Value<long>("enrollid");
                    var time = ss.Value<string>("time");
                    var mode = ss.Value<int>("mode");
                    var statusCode = ss.Value<int>("event");
                    var image = ss.Value<string>("image");
                    if (userId > 0)
                    {
                        var timeConverted = DateTime.Parse(time, new CultureInfo("en-US"));
                        var attendance = new DtoAttendance
                        {
                            EmployeeNumber = userId,
                            AttendanceDateTime = timeConverted,
                            VerificationStyle = (int)TimyUtils.GetVerificationStyle(mode),
                            AttendanceSource = AttendanceSourceEnumeration.Device,
                            DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                            DeviceNumber = DeviceInfo.DeviceNumber,
                            CameraId = null,
                            StatusCode = statusCode,
                            IsSent = false,
                            IsInvalid = false,
                            Id = logIndex.ToInt64(),
                            RfCardNumber = null,
                        };

                        if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.ServerRealTimeAttendance))
                        {
                            LoggingSystem.LogInfo("Timy Server Real Time Attendance:", attendance);
                        }

                        HardwareEventPublisher.Instance.PublishAttendance(attendance);
                        try
                        {
                            if (image.IsNotNullOrEmpty())
                            {
                                HardwareEventPublisher.Instance.PublishAttendanceImage(new DtoDeviceAttendanceImage
                                {
                                    DeviceNumber = DeviceInfo.DeviceNumber,
                                    EmployeeNumber = attendance.EmployeeNumber,
                                    AttendanceDateTime = attendance.AttendanceDateTime,
                                    Image = Convert.FromBase64String(image)
                                });
                            }
                        }
                        catch (Exception exp)
                        {
                            LoggingSystem.LogError(exp, "Error on TimyDeviceAgent.ProcessAttendanceLog process image");
                        }
                        
                    }
                    else
                    {
                        //event
                    }
                }

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on TimyDeviceAgent.ProcessAttendanceLog");
            }
            finally
            {
                Send(ObjectHelper.SerializeAsJson(new
                {
                    ret = "sendlog",
                    result = true,
                    cloudtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    access = 1,
                    message = ""
                }));
            }
            
        }

        private bool ProcessReadAttendanceLog(JObject jsonMessage, string commandName)
        {
            var continueWait = false;
            var count = 0;
            var indexTo = 0;
            try
            {

                count = jsonMessage.Value<int>("count");
                indexTo = jsonMessage.Value<int>("to");
                var attRecords = jsonMessage["record"];

                foreach (var ss in attRecords)
                {
                    var userId = ss.Value<long>("enrollid");
                    var time = ss.Value<string>("time");
                    var mode = ss.Value<int>("mode");
                    var statusCode = ss.Value<int>("event");
                    var image = ss.Value<string>("image");

                    var timeConverted = DateTime.Parse(time, new CultureInfo("en-US"));
                    var attendance = new DtoAttendance
                    {
                        EmployeeNumber = userId,
                        AttendanceDateTime = timeConverted,
                        VerificationStyle = (int)TimyUtils.GetVerificationStyle(mode),
                        AttendanceSource = AttendanceSourceEnumeration.Device,
                        DeviceAttendanceIoRetrieveType = DeviceAttendanceIoRetrieveTypeEnumeration.Push,
                        DeviceNumber = DeviceInfo.DeviceNumber,
                        CameraId = null,
                        StatusCode = statusCode,
                        IsSent = false,
                        IsInvalid = false,
                        RfCardNumber = null,
                    };

                    if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.GetData))
                    {
                        LoggingSystem.LogInfo("Timy Server Get Data Attendance:", attendance);
                    }

                    if (DeviceInfo.DeviceSettings.HasFlag(DeviceSettingsEnumeration.DontSaveAttendance))
                    {
                        continue;
                    }

                    HardwareEventPublisher.Instance.PublishAttendance(attendance);

                    try
                    {
                        if (image.IsNotNullOrEmpty())
                        {
                            HardwareEventPublisher.Instance.PublishAttendanceImage(new DtoDeviceAttendanceImage
                            {
                                DeviceNumber = DeviceInfo.DeviceNumber,
                                EmployeeNumber = attendance.EmployeeNumber,
                                AttendanceDateTime = attendance.AttendanceDateTime,
                                Image = Convert.FromBase64String(image)
                            });
                        }
                    }
                    catch (Exception exp)
                    {
                        LoggingSystem.LogError(exp, "Error on TimyDeviceAgent.ProcessReadAttendanceLog process image");
                    }

                }
            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on TimyDeviceAgent.ProcessReadAttendanceLog");
            }
            finally
            {
                if (indexTo < count)
                {
                    Send(ObjectHelper.SerializeAsJson(new
                    {
                        ret = commandName,
                        stn = false,
                    }));
                    continueWait = true;
                }
            }
            return continueWait;

        }

        private void ProcessUserData(JObject jsonMessage, bool sendAck)
        {
            var userId = 0;
            var backupNumber = 0;
            try
            {
                userId = jsonMessage.Value<int>("enrollid");
                backupNumber = jsonMessage.Value<int>("backupnum");
                var username = jsonMessage.Value<string>("name");
                var admin = jsonMessage.Value<int>("admin");
                var userInfo = new DtoEmployeeDeviceRelatedData
                {
                    EmployeeNumber = userId,
                    UserName = username,
                    Privilege = admin,
                    IsEnable = true,
                };

                if (backupNumber >= 20 && backupNumber <= 27)
                {
                    var fingerData = jsonMessage.Value<string>("record");
                    var bytes = Convert.FromBase64String(fingerData);
                    var face = new DtoEmployeeFace
                    {
                        EmployeeNumber = userId,
                        FaceIndex = backupNumber - 20,
                        TemplateData = bytes,
                        Length = bytes.Length,
                    };
                    HardwareEventPublisher.Instance.PublishNewFaceEnrolled(face, DeviceInfo.DeviceNumber);
                }
                else if (backupNumber >= 0 && backupNumber <= 9)
                {
                    var fingerData = jsonMessage.Value<string>("record");
                    if (fingerData.IsNotNullOrEmpty())
                    {
                        var bytes = Encoding.UTF8.GetBytes(fingerData);
                        var finger = new DtoEmployeeFinger
                        {
                            EmployeeNumber = userId,
                            FingerIndex = backupNumber,
                            TemplateData = bytes
                        };
                        HardwareEventPublisher.Instance.PublishNewFingerEnrolled(finger, DeviceInfo.DeviceNumber);
                    }
                }
                else if (backupNumber == 10)
                {
                    userInfo.Password = jsonMessage.Value<int>("record").ToString();
                    HardwareEventPublisher.Instance.PublishNewUserEnrolled(userInfo, DeviceInfo.DeviceNumber, new DtoEmployeeEnrolledSetting
                    {
                        OverwriteDevicePassword = true,
                        OverwriteIsEnabled = false,
                        OverwritePrivilege = false,
                        OverwriteRfCardNumber = false,
                        OverwriteVerificationStyle = false,
                    });
                }
                else if (backupNumber == 11)
                {
                    userInfo.RfCardNumbers = new List<string> { jsonMessage.Value<string>("record") };
                    HardwareEventPublisher.Instance.PublishNewUserEnrolled(userInfo, DeviceInfo.DeviceNumber, new DtoEmployeeEnrolledSetting
                    {
                        OverwriteDevicePassword = false,
                        OverwriteIsEnabled = false,
                        OverwritePrivilege = false,
                        OverwriteRfCardNumber = true,
                        OverwriteVerificationStyle = false,
                    });
                }
                else if (backupNumber == 40 || backupNumber == 41)
                {
                    var palmBase64 = jsonMessage.Value<string>("record");
                    if (palmBase64.IsNotNullOrEmpty())
                    {
                        var palm = new DtoEmployeePalm
                        {
                            EmployeeNumber = userId,
                            TemplateData = Convert.FromBase64String(palmBase64),
                            Index = backupNumber,
                        };
                        HardwareEventPublisher.Instance.PublishNewPalmEnrolled(palm, DeviceInfo.DeviceNumber);
                    }
                }
                else if (backupNumber == 50)
                {
                    var visibleBase64 = jsonMessage.Value<string>("record");
                    if (visibleBase64.IsNotNullOrEmpty())
                    {
                        var face = new DtoEmployeeFace
                        {
                            EmployeeNumber = userId,
                            FaceIndex = backupNumber,
                            TemplateData = Convert.FromBase64String(visibleBase64)
                        };
                        HardwareEventPublisher.Instance.PublishNewFaceEnrolled(face, DeviceInfo.DeviceNumber);
                    }
                }
                else
                {
                    HardwareEventPublisher.Instance.PublishNewUserEnrolled(userInfo, DeviceInfo.DeviceNumber, new DtoEmployeeEnrolledSetting
                    {
                        OverwriteDevicePassword = false,
                        OverwriteIsEnabled = false,
                        OverwritePrivilege = false,
                        OverwriteRfCardNumber = false,
                        OverwriteVerificationStyle = false,
                    });
                }

            }
            catch (Exception exp)
            {
                LoggingSystem.LogError(exp, "Error on TimyDeviceAgent.ProcessUserData");
            }
            finally
            {

                if (sendAck)
                {
                    Send(ObjectHelper.SerializeAsJson(new
                    {
                        ret = "senduser",
                        result = true,
                        enrollid = userId,
                        backupnum = backupNumber
                    }));
                }
            }

        }

        /// <summary>
        /// دستور را می‌فرستد و تا رسیدن Ack یا تایم‌اوت (هرکدام زودتر) صبر می‌کند.
        /// اگر دستگاه مشغول دستور قبلی باشد، فوراً false برمی‌گرداند (no-op، صف یا نگه‌داری دستور رد شده‌ای انجام نمی‌شود).
        /// این متد خودش منتظر نتیجه می‌ماند؛ caller (TimyServer) چندین دستگاه را با Task.WhenAll موازی صدا می‌زند.
        /// </summary>
        public async Task<bool> TrySendCommandAsync(DtoDeviceUnsentCommand command, CancellationToken ct = default)
        {
            // متغیری که قرار است نتیجه‌ی نهایی (Ack دستگاه یا Timeout) را در خود نگه دارد
            TaskCompletionSource<CommandAckResult> tcs;

            // قفل کوتاه فقط برای چک و ست کردن busy/idle به‌صورت atomic
            lock (_stateLock)
            {
                // اگر از قبل دستوری ارسال شده است (منتظر Ack هستیم)، این دستور را قبول نمی‌کنیم
                if (_pendingAck != null)
                    return false; // مشغول دستور قبلی؛ این دستور نادیده گرفته می‌شود

                // TaskCompletionSource پلی است بین event دریافت پاسخ از سوکت و این متد async
                // RunContinuationsAsynchronously یعنی ادامه‌ی کد بعد از await روی Thread Pool ادامه پیدا کند
                // (نه روی همان Thread‌ای که SetResult را صدا می‌زند، که می‌تواند Thread خود سوکت باشد)
                tcs = new TaskCompletionSource<CommandAckResult>(TaskCreationOptions.RunContinuationsAsynchronously);

                // ثبت در فیلد مشترک کلاس تا HandleCommandAcknowledgement بعداً پیدایش کند
                _pendingAck = tcs;
                //_pendingCommand = command;
            }

            try
            {
                // ارسال واقعی روی WebSocket
                Send(command.CommandContent);

                if (AppConfigs.LogLevelTimy.HasFlag(LogLevelTimyEnumeration.ServerMessage))
                {
                    LoggingSystem.LogInfo($"Command sent to [{DeviceInfo.SerialNumber}], CommandId={command.Id}");
                }

                // اعلام به دیتابیس که دستور ارسال شد (قبل از اینکه بدانیم جوابش چیست)
                HardwareEventPublisher.Instance.PublishCommandSentToDevice(new List<int> { command.Id });
            }
            catch (Exception exp)
            {
                // اگر خود ارسال (نه جواب) با خطا مواجه شد، فوراً آزاد می‌شویم
                LoggingSystem.LogError(exp, $"Error sending command to device {DeviceInfo.SerialNumber}");
                ClearPending();
                return true; // دستور "مصرف" شد (تلاش برای ارسال انجام شد)، حتی اگر ناموفق
            }

            CommandAckResult result;

            // timeoutCts: کنترل‌کننده‌ی تایم‌اوت داخلی این دستور
            // linkedCts: ترکیب توکن کنسل بیرونی (ct) با تایم‌اوت داخلی، تا هرکدام زودتر فعال شد کار را متوقف کند
            using (var timeoutCts = new CancellationTokenSource())
            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token))
            {
                try
                {
                    // این یک Task است که فقط بعد از CommandResponseTimeout کامل می‌شود
                    // (مگر اینکه linkedCts قبل از آن کنسل شود)
                    var delayTask = Task.Delay(GetCommandTimeoutInMillisecond(command), linkedCts.Token);

                    // race بین رسیدن Ack از سوکت (tcs.Task) و سپری‌شدن Timeout (delayTask)
                    // هرکدام زودتر تمام شود، WhenAny همان را برمی‌گرداند
                    var completedTask = await Task.WhenAny(tcs.Task, delayTask);

                    if (completedTask == tcs.Task)
                    {
                        // جواب از دستگاه زودتر رسید؛ دیگر نیازی به delayTask نیست
                        // کنسلش می‌کنیم تا فوراً آزاد شود و بی‌خودی منتظر تایم‌اوتش نمانیم
                        timeoutCts.Cancel();

                        // چون tcs.Task از قبل کامل شده، این await فوری برمی‌گردد
                        result = await tcs.Task;
                    }
                    else if (ct.IsCancellationRequested)
                    {
                        // delayTask زودتر تمام شد، و دلیلش کنسل‌شدن توکن بیرونی بود (نه گذشت زمان)
                        result = new CommandAckResult
                        {

                            ErrorMessage = "Command send cancelled",
                            IsSuccessful = false,
                            ResponseTime = DateTime.Now,
                        };
                    }
                    else
                    {
                        // delayTask زودتر تمام شد و توکن بیرونی هم کنسل نشده؛ یعنی واقعاً زمان تمام شد
                        LoggingSystem.LogInfo($"Command timeout. CommandId={command.Id}, Device={DeviceInfo.SerialNumber}");
                        result = new CommandAckResult
                        {

                            ErrorMessage = "Command timeout",
                            IsSuccessful = false,
                            ResponseTime = DateTime.Now,
                        };
                    }
                }
                finally
                {
                    // در هر حالت (جواب، تایم‌اوت، یا کنسل) دستگاه باید دوباره آزاد شود
                    // تا دور بعدی Timer بتواند دستور جدیدی به همین Session بدهد
                    ClearPending();
                }
            }

            // نتیجه‌ی نهایی (موفق/ناموفق + پیام خطا در صورت وجود) در دیتابیس ثبت می‌شود
            if (result.IsSuccessful)
            {
                HardwareEventPublisher.Instance.PublishCommandResponseReceived(new DtoDeviceCommandProcessingResult
                {
                    Id = command.Id,
                    CommandResponseTime = result.ResponseTime,
                    CommandResponseResult = "SUCCESS"
                });
            }
            else
            {
                HardwareEventPublisher.Instance.PublishCommandDescriptionReceived(
                    new DtoDeviceCommandProcessingDescription
                    {
                        Id = command.Id,
                        Description = result.ErrorMessage,
                    });
            }

            return true;
        }

        private void ClearPending()
        {
            lock (_stateLock)
            {
                _pendingAck = null;
                //_pendingCommand = null;
            }
        }

        private void Send(string payload)
        {
            // SuperSocket session.Send thread-safe کامل نیست؛ قفل سبک برای احتیاط
            lock (_sendLock)
            {
                _socketSession.Send(payload);
            }
        }

        private int GetCommandTimeoutInMillisecond(DtoDeviceUnsentCommand command)
        {
            switch (command.CommandType)
            {
                case DeviceCommandTypeEnumeration.ReadAttendance:
                case DeviceCommandTypeEnumeration.ReadoutAttendance:
                case DeviceCommandTypeEnumeration.ScanFace:
                case DeviceCommandTypeEnumeration.ScanFinger:
                case DeviceCommandTypeEnumeration.ScanCard:
                case DeviceCommandTypeEnumeration.ScanIris:
                    return _pushConfig.LongCommandTimeoutInSecond * 1000;
                default:
                    return _pushConfig.NormalCommandTimeoutInSecond * 1000;
            }
        }

        #region Implementation of IDisposable

        private bool _disposed;

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
            }
            lock (_stateLock)
            {
                _pendingAck?.TrySetCanceled();
                _pendingAck = null;
            }
            _disposed = true;
        }

        ~TimyDeviceAgent()
        {
            Dispose(false);
        }

        #endregion




        private class CommandAckResult
        {
            public DateTime ResponseTime { get; set; }
            public bool IsSuccessful { get; set; }
            public string ErrorMessage { get; set; }
        }

    }
}