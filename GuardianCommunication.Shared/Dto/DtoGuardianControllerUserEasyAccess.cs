using System;

namespace GuardianCommunication.Shared.Dto
{
    /// <summary>
    /// دسترسی آسان کاربر روی برد (همراه اطلاعات کاربر)
    /// </summary>
    public class DtoGuardianControllerUserEasyAccess
    {
        /// <summary>
        /// شناسه دسترسی آسان
        /// </summary>
        public Guid EasyAccessId { get; set; }

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
