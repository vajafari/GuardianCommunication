namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// تنظیمات سرور gRPC بورد کنترلر گاردین
    /// </summary>
    public class GuardianControllerServerConfig
    {
        /// <summary>
        /// درگاه سرور gRPC
        /// </summary>
        public int GrpcServerPort { get; set; }

        /// <summary>
        /// تعداد دستور خوانده‌شده برای هر دستگاه در هر دور
        /// </summary>
        public int GrpcServerCommandCount { get; set; }

        /// <summary>
        /// فاصله دورهای ارسال دستور
        /// </summary>
        public int GrpcCommandTimerIntervalInMillisecond { get; set; }

        /// <summary>
        /// زمان انتظار پاسخ دستورهای کوتاه
        /// </summary>
        public int GrpcServerTimeoutShortInMillisecond { get; set; }

        /// <summary>
        /// زمان انتظار پاسخ دستورهای طولانی
        /// </summary>
        public int GrpcServerTimeoutLongInMillisecond { get; set; }

        /// <summary>
        /// زمان انتظار پاسخ دستورهای خیلی طولانی
        /// </summary>
        public int GrpcServerTimeoutVeryLongInMillisecond { get; set; }
    }
}
