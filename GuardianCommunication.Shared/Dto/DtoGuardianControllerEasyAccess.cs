using System;

namespace GuardianCommunication.Shared.Dto
{
    /// <summary>
    /// یک دسترسی آسان شخص برای ارسال جداگانه روی برد کنترلر گاردین
    /// </summary>
    public class DtoGuardianControllerEasyAccess
    {
        /// <summary>
        /// شناسه دسترسی آسان
        /// </summary>
        public Guid EasyAccessId { get; set; }

        /// <summary>
        /// شماره کاربر روی برد
        /// </summary>
        public long UserIdOnDevice { get; set; }

        public Guid AccessLevelId { get; set; }

        public Guid? CalendarId { get; set; }

        /// <summary>
        /// شروع اعتبار (زمان محلی)
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// پایان اعتبار (زمان محلی)؛ خالی یعنی بدون پایان (دسترسی آسانی که فقط سقف تردد دارد)
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// سقف تعداد تردد
        /// </summary>
        public int? AttendanceCount { get; set; }
    }
}
