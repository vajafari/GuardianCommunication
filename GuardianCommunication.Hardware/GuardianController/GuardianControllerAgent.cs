using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuardianCommunication.Data.Logger;
using GuardianCommunication.Hardware.GuardianController.Definition;
using GuardianCommunication.Hardware.GuardianController.Model;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.SharedSettings;
using Grpc.Core;
using Guardian.Controller;

namespace GuardianCommunication.Hardware.GuardianController
{
    /// <summary>
    /// مسئول مدیریت تمام تعاملات بلادرنگ با یک برد کنترلر گاردین.
    /// به محض اتصال برد، یک نمونه از این کلاس ساخته می‌شود.
    /// در هر لحظه فقط یک دستور در حال ارسال است (single in-flight)، وضعیت busy/idle
    /// توسط خود Agent نگه‌داری می‌شود و Server قبل از fetch آن را از طریق <see cref="IsIdle"/> می‌پرسد.
    /// </summary>
    public sealed class GuardianControllerAgent : IDisposable
    {
        public DtoDevice DeviceInfo { get; internal set; }
        private readonly IServerStreamWriter<GuardianControllerGrpcMessage> _writer;
        private readonly IAsyncStreamReader<GuardianControllerGrpcMessage> _reader;
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
        // پل مشترک بین فرستنده و گیرنده + نشانگر busy/idle
        // ====================================================================
        // وقتی دستوری ارسال می‌شود یک TaskCompletionSource ساخته و اینجا ثبت می‌شود؛
        // وقتی پاسخ برد (با همان شناسه پیام) در حلقه‌ی دریافت می‌رسد، همان TCS کامل می‌شود.
        // تا وقتی این فیلد null نشده باشد برد «مشغول» است و دستور جدیدی نمی‌پذیرد.
        // پاک‌سازی این فیلد فقط در finally متد ارسال (ClearPending) انجام می‌شود، نه در گیرنده.
        private readonly object _pendingLock = new object();
        private TaskCompletionSource<GuardianControllerGrpcDeviceCommandResultModel> _pendingResponse;
        private string _pendingMessageId;

        private Task _processingTask;

        /// <summary>
        /// آیا این برد الان آزاد است (منتظر جواب دستور قبلی نیست)؟
        /// Server قبل از fetch از دیتابیس این را چک می‌کند تا فقط دستور بردهای آزاد را بخواند.
        /// </summary>
        public bool IsIdle
        {
            get { lock (_pendingLock) return _pendingResponse == null; }
        }

        public GuardianControllerAgent(
            DtoDevice deviceInfo
            , IServerStreamWriter<GuardianControllerGrpcMessage> writer
            , IAsyncStreamReader<GuardianControllerGrpcMessage> reader
            , CancellationToken token
            , Func<DtoServerMatchData, DtoServerMatchResult> serverMatchProcessor)
        {
            DeviceInfo = deviceInfo;
            _writer = writer;
            _reader = reader;
            AbortToken = token;
            _serverMatchProcessor = serverMatchProcessor;
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.GrpcCreateAgent))
            {
                LoggingSystem.LogInfo("New GuardianControllerAgent Created", new { deviceInfo.Id, deviceInfo.SerialNumber });
            }
        }

        #region Output Messages

        /// <summary>
        /// دستور را می‌فرستد و تا رسیدن پاسخ یا تایم‌اوت (هرکدام زودتر) صبر می‌کند.
        /// اگر برد مشغول دستور قبلی باشد، فوراً LineIsBusy برمی‌گرداند (بدون صف و بدون نگه‌داری دستور).
        /// </summary>
        public async Task<GuardianControllerGrpcDeviceCommandResultModel> SendCommandAsync(
            DtoDeviceUnsentCommand command, int timeoutInMillisecond)
        {
            if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.GrpcAgentSendCommand))
            {
                LoggingSystem.LogInfo("SendCommand Called", new
                {
                    command.DeviceId,
                    command.CommandType,
                    command.NumericId,
                    command.CommandContent
                });
            }

            // RunContinuationsAsynchronously: ادامه‌ی کد بعد از await روی ThreadPool اجرا شود،
            // نه روی همان تردی که TrySetResult را صدا می‌زند (که تردِ حلقه‌ی دریافت gRPC است).
            var tcs = new TaskCompletionSource<GuardianControllerGrpcDeviceCommandResultModel>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            // قفل کوتاه فقط برای چک‌کردن و ست‌کردن busy/idle به‌صورت atomic
            lock (_pendingLock)
            {
                // اگر از قبل دستوری در جریان است (منتظر پاسخ هستیم)، این دستور را نمی‌پذیریم
                if (_pendingResponse != null)
                {
                    if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.GrpcAgentSendCommand))
                    {
                        LoggingSystem.LogInfo("SendCommand- Line is busy detected", new
                        {
                            command.DeviceId,
                            command.CommandType,
                            command.NumericId,
                        });
                    }
                    return new GuardianControllerGrpcDeviceCommandResultModel
                    {
                        ErrorCode = GuardianControllerErrorEnumeration.LineIsBusy,
                    };
                }

                _pendingResponse = tcs;
                _pendingMessageId = command.NumericId.ToString();
            }

            try
            {
                var message = new GuardianControllerGrpcMessage
                {
                    ErrorCode = 0,
                    MessageId = command.NumericId.ToString(),
                    MessageContent = ObjectHelper.SerializeAsJson(new GuardianControllerGrpcDeviceCommandModel
                    {
                        command_content = command.CommandContent,
                        command_type = (int)command.CommandType,
                    }),
                    MessageType = GuardianControllerGrpcMessageType.SendCommand,
                    DeviceSerialNumber = DeviceInfo.SerialNumber,
                };

                if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.GrpcAgentSendCommand))
                {
                    LoggingSystem.LogInfo("SendCommand- System is ready to send command on line", new
                    {
                        command.DeviceId,
                        command.CommandType,
                        command.NumericId,
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
                    if (AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.GrpcAgentSendCommand))
                    {
                        LoggingSystem.LogInfo("SendCommand- message timeout", new
                        {
                            command.DeviceId,
                            command.CommandType,
                            command.NumericId,
                        });
                    }

                    // یا زمان تمام شد یا اتصال قطع شد؛ در هر دو حالت دستور «ارسال‌نشده» می‌ماند تا دور بعد دوباره تلاش شود
                    return new GuardianControllerGrpcDeviceCommandResultModel
                    {
                        ErrorCode = GuardianControllerErrorEnumeration.MessageTimeout,
                    };
                }
            }
            catch (OperationCanceledException)
            {
                // tcs.Task را Dispose (قطع اتصال/reconnect) کنسل کرده، نه یک خطای واقعی؛
                // مثل Timeout رفتار می‌کنیم تا دستور «ارسال‌نشده» بماند و دور بعد دوباره تلاش شود.
                return new GuardianControllerGrpcDeviceCommandResultModel
                {
                    ErrorCode = GuardianControllerErrorEnumeration.MessageTimeout,
                };
            }
            catch (Exception ex)
            {
                LoggingSystem.LogError(ex, "GuardianControllerAgent.SendCommand Unknown error");
                return new GuardianControllerGrpcDeviceCommandResultModel
                {
                    ErrorCode = GuardianControllerErrorEnumeration.UnknownError,
                    Message = ex.GetFullExceptionMessage(),
                };
            }
            finally
            {
                // در هر حالت (پاسخ، تایم‌اوت، خطا یا قطع اتصال) برد دوباره آزاد می‌شود
                ClearPending();
            }
        }

        /// <summary>
        /// نوشتن ایمن روی استریم با استفاده از _writeLock.
        /// این متد توسط هر دو بخش (ارسال دستور و ارسال ACK داخل حلقه‌ی دریافت) استفاده می‌شود.
        /// </summary>
        private async Task SafeWriteAsync(GuardianControllerGrpcMessage message)
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
                _pendingMessageId = null;
            }
        }

        #endregion

        #region Input Messages

        /// <summary>
        /// <paramref name="firstMessage"/>: پیامی که Server از قبل برای هندشیک (خواندن سریال) مصرف کرده است.
        /// چون آن پیام از استریم خارج شده و دیگر با MoveNext قابل خواندن نیست، باید همینجا هم
        /// مثل بقیه‌ی پیام‌ها Route شود.
        /// </summary>
        public async Task StartProcessingAsync(GuardianControllerGrpcMessage firstMessage = null)
        {
            _processingTask = Task.Run(async () =>
            {
                try
                {
                    if (firstMessage != null)
                    {
                        await RouteIncomingMessageAsync(firstMessage).ConfigureAwait(false);
                    }

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
                    LoggingSystem.LogError(exp, "Error on GuardianControllerAgent.StartProcessingAsync", new
                    {
                        Message = firstMessage
                    });
                }
            }, AbortToken);

            await _processingTask.ConfigureAwait(false);
        }

        private async Task RouteIncomingMessageAsync(GuardianControllerGrpcMessage msg)
        {
            if (msg.MessageType != GuardianControllerGrpcMessageType.Heartbeat
                && AppConfigs.LogLevelGuardianController.HasFlag(LogLevelGuardianControllerEnumeration.GrpcAgentReceiveCommand))
            {
                LoggingSystem.LogInfo("RouteIncomingMessage-Message Received", new
                {
                    Message = msg,
                    DeviceInfo.Id,
                });
            }

            var responseType = GuardianControllerGrpcMessageType.Ack;
            var messageBody = string.Empty;
            switch (msg.MessageType)
            {
                case GuardianControllerGrpcMessageType.ResponseCommand:
                    var messageResponse = ObjectHelper.DeserializeAsJson<GuardianControllerErrorCommunicationModel>(msg.MessageContent);
                    // فقط پاسخ را داخل TCS منتظر می‌ریزیم؛ پاک‌سازی _pendingResponse بر عهده‌ی ClearPending در finally متد ارسال است.
                    // جواب دیرهنگام دستوری که مهلتش تمام شده (شناسه دیگر) نادیده گرفته می‌شود
                    lock (_pendingLock)
                    {
                        if (_pendingMessageId == msg.MessageId)
                        {
                            _pendingResponse?.TrySetResult(new GuardianControllerGrpcDeviceCommandResultModel
                            {
                                ErrorCode = msg.ErrorCode == 0
                                    ? GuardianControllerErrorEnumeration.Success
                                    : GuardianControllerErrorEnumeration.CommandRejected,
                                DeviceSerialNumber = msg.DeviceSerialNumber,
                                CommandId = msg.MessageId.TryConvertToInt64(),
                                Message = GuardianControllerUtils.DescribeError(messageResponse),
                            });
                        }
                    }
                    break;
                case GuardianControllerGrpcMessageType.DeviceEvent:
                    {
                        var models = ObjectHelper.DeserializeAsJson<List<GuardianControllerDoorEventCommunicationModel>>(msg.MessageContent);
                        if (models.IsCollectionNotNullOrEmpty())
                        {
                            GuardianControllerUtils.ProcessDoorEvent(DeviceInfo, models);
                        }
                    }
                    break;
                case GuardianControllerGrpcMessageType.DeviceAttendance:
                    {
                        var models = ObjectHelper.DeserializeAsJson<List<GuardianControllerAttendanceCommunicationModel>>(msg.MessageContent);
                        if (models.IsCollectionNotNullOrEmpty())
                        {
                            GuardianControllerUtils.ProcessAttendance(DeviceInfo, models);
                        }
                    }
                    break;
                case GuardianControllerGrpcMessageType.SyncDatetime:
                    {
                        messageBody = ObjectHelper.SerializeAsJson(GuardianControllerUtils.MapDateAndTimeToCommunicationModel(DeviceInfo, DateTime.UtcNow));
                    }
                    break;
                case GuardianControllerGrpcMessageType.MatchOnServer:
                    {
                        var attendance = ObjectHelper.DeserializeAsJson<GuardianControllerAttendanceCommunicationModel>(msg.MessageContent)
                                         ?? new GuardianControllerAttendanceCommunicationModel();
                        var authorizationResult = _serverMatchProcessor(new DtoServerMatchData
                        {
                            DeviceId = DeviceInfo.Id,
                            RfCardNumber = attendance.rf_card_number,
                            EventDateTime = attendance.attendance_date_time.ToUtcDateTimeFromEpochMillisecondTime(),
                            UserIdOnDevice = attendance.user_id_on_device,
                            DoorId = attendance.door_id,
                        });

                        messageBody = ObjectHelper.SerializeAsJson(new GuardianControllerMatchOnServerResponseModel
                        {
                            is_authorize = authorizationResult != null && authorizationResult.IsSuccessfullyProcessed,
                            user_id_on_device = authorizationResult != null && authorizationResult.UserIdOnDevice != 0
                                ? authorizationResult.UserIdOnDevice
                                : attendance.user_id_on_device,
                        });
                        responseType = GuardianControllerGrpcMessageType.MatchOnServerResponse;
                    }
                    break;
                case GuardianControllerGrpcMessageType.Heartbeat:
                    break;
            }
            await SafeWriteAsync(new GuardianControllerGrpcMessage
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

            // اگر برد قطع شد، درخواست معلق را لغو کن تا تردِ منتظر بلافاصله آزاد شود (جلوگیری از Thread Leak)
            lock (_pendingLock)
            {
                _pendingResponse?.TrySetCanceled();
                _pendingResponse = null;
            }
            _disposed = true;
        }

        ~GuardianControllerAgent()
        {
            Dispose(false);
        }

        #endregion
    }
}
