using System;
using System.Diagnostics.CodeAnalysis;

namespace GuardianCommunication.Hardware.GuardianController.Model
{
    /// <summary>
    /// دسترسی آسان: درب‌های یک سطح دسترسی در یک بازه، با تقویم و سقف تردد اختیاری؛ عضو فهرست بدنه POST api/v1/easy-accesses/upsert
    /// در Standalone و محتوای دستور GuardianControllerSetEasyAccess در Push، و همراه کاربر در مدل کاربر.
    /// شمارنده تردد روی برد با شناسه همین دسترسی نگه داشته می‌شود و با ارسال دوباره صفر نمی‌شود
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GuardianControllerEasyAccessCommunicationModel
    {
        public Guid id { get; set; }

        /// <summary>
        /// شماره کاربر روی دستگاه
        /// </summary>
        public long user_id_on_device { get; set; }

        public Guid access_level_id { get; set; }
        public Guid? calendar_id { get; set; }

        /// <summary>
        /// شروع بازه به میلی‌ثانیه epoch (UTC)
        /// </summary>
        public long start_date { get; set; }

        /// <summary>
        /// پایان بازه به میلی‌ثانیه epoch (UTC)؛ خالی یعنی بدون پایان
        /// </summary>
        public long? end_date { get; set; }

        public int? attendance_count { get; set; }
    }
}
