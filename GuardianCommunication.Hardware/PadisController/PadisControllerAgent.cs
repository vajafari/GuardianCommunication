using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.PadisController.Definition;
using GuardianCommunication.Hardware.PadisController.Model;
using Padis.Controller;

namespace GuardianCommunication.Hardware.PadisController
{
    /// <summary>
    /// مسئول مدیریت تمام تعاملات بلادرنگ با یک کنترلر خاص.
    /// به محض اتصال دستگاه، یک نمونه از این کلاس ساخته می‌شود.
    /// معماری آن دقیقاً از TimyDeviceAgent الگو گرفته است:
    /// در هر لحظه فقط یک دستور در حال ارسال است (single in-flight)، وضعیت busy/idle
    /// توسط خود Agent نگه‌داری می‌شود و Server قبل از fetch آن را از طریق <see cref="IsIdle"/> می‌پرسد.
    /// </summary>
    public sealed class PadisControllerAgent : IDisposable
    {
        public DtoCommunicationDeviceData DeviceInfo { get; }
        private readonly IServerStreamWriter<PadisControllerGrpcMessage> _writer;
        private readonly IAsyncStreamReader<PadisControllerGrpcMessage> _reader;
        public CancellationToken AbortToken { get; }
        private readonly Func<DtoServerMatchData, DtoServerMatchResult> _serverMatchProcessor;

        // ====================================================================
        // قفل ارسال روی استریم (_writeLock)
        // ====================================================================
        // کتابخانه Grpc.Core در نوشتن هم‌زمان روی یک استریم Thread-Safe نیست؛
        // این قفل باید توسط «همه» (هم ارسال دستور و هم ارسال ACK داخل حلقه‌ی دریافت) استفاده شود.
        // فقط لحظه‌ی نوشتن را محافظت می‌کند، نه کل زمان انتظار پاسخ را.
        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1, 1);

        // ====================================================================
        // پل مشترک بین فرستنده و گیرنده + نشانگر busy/idle  (هم‌سان با TimyDeviceAgent._pendingAck)
        // ====================================================================
        // وقتی دستوری ارسال می‌شود یک TaskCompletionSource ساخته و اینجا ثبت می‌شود؛
        // وقتی پاسخ دستگاه در حلقه‌ی دریافت می‌رسد، همان TCS کامل می‌شود.
        // تا وقتی این فیلد null نشده باشد دستگاه «مشغول» است و دستور جدیدی نمی‌پذیرد.
        // پاک‌سازی این فیلد فقط در finally متد ارسال (ClearPending) انجام می‌شود، نه در گیرنده؛
        // این تضمین می‌کند که تا پایان کامل عملیات ارسال، Agent مشغول باقی می‌ماند و از
        // شرایط مسابقه (race) بین «آزاد شدن زودهنگام» و «ثبت دستور بعدی» جلوگیری می‌شود.
        private readonly object _pendingLock = new object();
        private TaskCompletionSource<PadisControllerGrpcDeviceCommandResultModel> _pendingResponse;

        private Task _processingTask;

        /// <summary>
        /// آیا این دستگاه الان آزاد است (منتظر جواب دستور قبلی نیست)؟
        /// Server قبل از fetch از دیتابیس این را چک می‌کند تا فقط دستور دستگاه‌های آزاد را بخواند.
        /// </summary>
        public bool IsIdle
        {
            get { lock (_pendingLock) return _pendingResponse == null; }
        }

        public PadisControllerAgent(
            DtoCommunicationDeviceData deviceInfo
            , IServerStreamWriter<PadisControllerGrpcMessage> writer
            , IAsyncStreamReader<PadisControllerGrpcMessage> reader
            , CancellationToken token
            , Func<DtoServerMatchData, DtoServerMatchResult> serverMatchProcessor)
        {
            DeviceInfo = deviceInfo;
            _writer = writer;
            _reader = reader;
            AbortToken = token;
            _serverMatchProcessor = serverMatchProcessor;
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.GrpcCreateAgent))
            {
                LoggingSystem.LogInfo("New PadisControllerAgent Created", DeviceInfo = deviceInfo);
            }
        }

        #region Output Messages

        /// <summary>
        /// نسخه‌ی هم‌زمان (blocking) برای حفظ سازگاری با API قبلی.
        /// ارسال واقعی و انتظار برای پاسخ در <see cref="SendCommandAsync"/> انجام می‌شود.
        /// </summary>
        public PadisControllerGrpcDeviceCommandResultModel SendCommand(DtoDeviceUnsentCommand command, int timeoutInMillisecond)
        {
            return SendCommandAsync(command, timeoutInMillisecond).GetAwaiter().GetResult();
        }

        /// <summary>
        /// دستور را می‌فرستد و تا رسیدن پاسخ یا تایم‌اوت (هرکدام زودتر) صبر می‌کند.
        /// اگر دستگاه مشغول دستور قبلی باشد، فوراً LineIsBusy برمی‌گرداند (بدون صف و بدون نگه‌داری دستور).
        /// این متد کاملاً async است؛ Server چند دستگاه را به‌صورت موازی (fire-and-forget) صدا می‌زند.
        /// </summary>
        public async Task<PadisControllerGrpcDeviceCommandResultModel> SendCommandAsync(
            DtoDeviceUnsentCommand command, int timeoutInMillisecond)
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.GrpcAgentSendCommand))
            {
                LoggingSystem.LogInfo("Padis controller agent Command Request received", new
                {
                    command.DeviceNumber,
                    command.CommandType
                });
            }

            TaskCompletionSource<PadisControllerGrpcDeviceCommandResultModel> tcs;

            // قفل کوتاه فقط برای چک‌کردن و ست‌کردن busy/idle به‌صورت atomic
            lock (_pendingLock)
            {
                // اگر از قبل دستوری در جریان است (منتظر پاسخ هستیم)، این دستور را نمی‌پذیریم
                if (_pendingResponse != null)
                {
                    if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.GrpcAgentSendCommand))
                    {
                        LoggingSystem.LogInfo("Padis controller agent Command Request received- Line is busy", new
                        {
                            command.DeviceNumber,
                            command.CommandType
                        });
                    }
                    return new PadisControllerGrpcDeviceCommandResultModel
                    {
                        ErrorCode = PadisControllerErrorEnumeration.LineIsBusy,
                    };
                }

                // RunContinuationsAsynchronously: ادامه‌ی کد بعد از await روی ThreadPool اجرا شود،
                // نه روی همان تردی که TrySetResult را صدا می‌زند (که تردِ حلقه‌ی دریافت gRPC است).
                tcs = new TaskCompletionSource<PadisControllerGrpcDeviceCommandResultModel>(
                    TaskCreationOptions.RunContinuationsAsynchronously);
                _pendingResponse = tcs;
            }

            try
            {
                var message = new PadisControllerGrpcMessage
                {
                    ErrorCode = 0,
                    MessageId = command.Id.ToString(),
                    MessageContent = ObjectHelper.SerializeAsJson(new PadisControllerGrpcDeviceCommandModel
                    {
                        CommandContent = command.CommandContent,
                        CommandType = (int)command.CommandType,
                    }),
                    MessageType = PadisControllerGrpcMessageType.SendCommand,
                    DeviceSerialNumber = DeviceInfo.SerialNumber,
                };

                if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.GrpcAgentSendCommand))
                {
                    LoggingSystem.LogInfo("Padis controller agent Command Request received-Message Created", new
                    {
                        command.DeviceNumber,
                        command.CommandType,
                    });
                }

                // ارسال واقعی روی استریم
                await SafeWriteAsync(message).ConfigureAwait(false);

                // timeoutCts: تایم‌اوت داخلی این دستور، linkedCts: ترکیب توکن قطع اتصال (AbortToken) با تایم‌اوت
                using (var timeoutCts = new CancellationTokenSource())
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(AbortToken, timeoutCts.Token))
                {
                    var delayTask = Task.Delay(timeoutInMillisecond, linkedCts.Token);

                    // مسابقه بین رسیدن پاسخ (tcs.Task) و سپری‌شدن زمان (delayTask)
                    var completed = await Task.WhenAny(tcs.Task, delayTask).ConfigureAwait(false);

                    if (completed == tcs.Task)
                    {
                        // پاسخ زودتر رسید؛ delayTask را کنسل می‌کنیم تا بی‌خود منتظر نماند
                        timeoutCts.Cancel();
                        return await tcs.Task.ConfigureAwait(false);
                    }

                    // یا زمان تمام شد یا اتصال قطع شد؛ در هر دو حالت دستور «ارسال‌نشده» می‌ماند تا دور بعد دوباره تلاش شود
                    return new PadisControllerGrpcDeviceCommandResultModel
                    {
                        ErrorCode = PadisControllerErrorEnumeration.MessageTimeout,
                    };
                }
            }
            catch (Exception ex)
            {
                LoggingSystem.LogError(ex, "Padis controller agent Command Request received-Error happened in process");
                return new PadisControllerGrpcDeviceCommandResultModel
                {
                    ErrorCode = PadisControllerErrorEnumeration.UnknownError,
                    Message = ex.GetFullExceptionMessage(),
                };
            }
            finally
            {
                // در هر حالت (پاسخ، تایم‌اوت، خطا یا قطع اتصال) دستگاه دوباره آزاد می‌شود
                ClearPending();
            }
        }

        /// <summary>
        /// نوشتن ایمن روی استریم با استفاده از _writeLock.
        /// این متد توسط هر دو بخش (ارسال دستور و ارسال ACK داخل حلقه‌ی دریافت) استفاده می‌شود.
        /// </summary>
        private async Task SafeWriteAsync(PadisControllerGrpcMessage message)
        {
            await _writeLock.WaitAsync(AbortToken).ConfigureAwait(false);
            try
            {
                if (!AbortToken.IsCancellationRequested)
                {
                    await _writer.WriteAsync(message).ConfigureAwait(false);
                }
            }
            finally
            {
                _writeLock.Release();
            }
        }

        private void ClearPending()
        {
            lock (_pendingLock)
            {
                _pendingResponse = null;
            }
        }

        #endregion

        #region Input Messages

        public async Task StartProcessingAsync()
        {
            _processingTask = Task.Run(async () =>
            {
                try
                {
                    while (await _reader.MoveNext(AbortToken).ConfigureAwait(false))
                    {
                        await RouteIncomingMessageAsync(_reader.Current).ConfigureAwait(false);
                    }
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
                {
                    // قطع اتصال نرمال
                }
                catch (OperationCanceledException)
                {
                    // قطع اتصال / توقف سرور
                }
                catch (Exception exp)
                {
                    LoggingSystem.LogError(exp, "Error on PadisControllerAgent.StartProcessingAsync");
                }
            }, AbortToken);

            await _processingTask.ConfigureAwait(false);
        }

        private async Task RouteIncomingMessageAsync(PadisControllerGrpcMessage msg)
        {
            if (AppConfigs.LogLevelPadisController.HasFlag(LogLevelPadisControllerEnumeration.GrpcAgentReceiveCommand))
            {
                LoggingSystem.LogInfo("Padis controller agent Command Request received-Message Created", msg);
            }

            var responseType = PadisControllerGrpcMessageType.Ack;
            var messageBody = string.Empty;
            switch (msg.MessageType)
            {
                case PadisControllerGrpcMessageType.ResponseCommand:
                    var messageResponse = ObjectHelper.DeserializeAsJson<PadisControllerGrpcDeviceCommandResultModel>(msg.MessageContent);
                    // فقط پاسخ را داخل TCS منتظر می‌ریزیم؛ پاک‌سازی _pendingResponse بر عهده‌ی
                    // ClearPending در finally متد ارسال است (هم‌سان با TimyDeviceAgent).
                    lock (_pendingLock)
                    {
                        _pendingResponse?.TrySetResult(new PadisControllerGrpcDeviceCommandResultModel
                        {
                            ErrorCode = (PadisControllerErrorEnumeration)msg.ErrorCode,
                            DeviceSerialNumber = msg.DeviceSerialNumber,
                            CommandId = msg.MessageId.TryConvertToInt32(),
                            Message = messageResponse.Message,
                        });
                    }
                    break;
                case PadisControllerGrpcMessageType.DeviceEvent:
                    {
                        var models = ObjectHelper.DeserializeAsJson<PadisControllerOperationLogCommunicationModel[]>(msg.MessageContent);
                        if (models.IsCollectionNotNullOrEmpty())
                        {
                            PadisControllerUtils.ProcessOperationLog(DeviceInfo, models);
                        }
                    }
                    break;
                case PadisControllerGrpcMessageType.DeviceAttendance:
                    {
                        var models = ObjectHelper.DeserializeAsJson<PadisControllerAttendanceCommunicationModel[]>(msg.MessageContent);
                        if (models.IsCollectionNotNullOrEmpty())
                        {
                            PadisControllerUtils.ProcessAttendance(DeviceInfo, models);
                        }
                    }
                    break;
                case PadisControllerGrpcMessageType.MatchOnServer:
                    {
                        var attendance = ObjectHelper.DeserializeAsJson<PadisControllerAttendanceCommunicationModel>(msg.MessageContent);
                        var authorizationResult = _serverMatchProcessor(new DtoServerMatchData
                        {
                            DeviceNumber = DeviceInfo.DeviceNumber,
                            RfCardNumber = attendance.RfCardNumber,
                            MatchType = ServerMatchingTypeEnumeration.CheckAccess,
                            EventDateTime = attendance.AttendanceDateTime.ToDateTimeFromEpochMillisecondTime(true),
                            Password = null,
                            TemplateData = null,
                            UserId = attendance.EmployeeNumber,
                        });
                        messageBody = ObjectHelper.SerializeAsJson(new PadisControllerMatchOnServerResponseModel
                        {
                            IsAuthorized = authorizationResult.IsSuccessfullyProcessed
                        });
                        responseType = PadisControllerGrpcMessageType.MatchOnServerResponse;
                    }
                    break;
                case PadisControllerGrpcMessageType.Heartbeat:
                    break;
            }
            await SafeWriteAsync(new PadisControllerGrpcMessage
            {
                DeviceSerialNumber = msg.DeviceSerialNumber,
                ErrorCode = 0,
                MessageId = msg.MessageId,
                MessageType = responseType,
                MessageContent = messageBody
            }).ConfigureAwait(false);
        }

        #endregion

        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                // Free any other managed objects here.
            }
            _writeLock.Dispose();

            // اگر دستگاه قطع شد، درخواست معلق را لغو کن تا تردِ منتظر بلافاصله آزاد شود (جلوگیری از Thread Leak)
            lock (_pendingLock)
            {
                _pendingResponse?.TrySetCanceled();
                _pendingResponse = null;
            }
            _disposed = true;
        }

        ~PadisControllerAgent()
        {
            Dispose(false);
        }

        #endregion
    }
}
