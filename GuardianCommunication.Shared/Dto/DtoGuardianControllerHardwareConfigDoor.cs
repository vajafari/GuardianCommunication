using System;
using System.Collections.Generic;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
    /// <summary>
    /// یک درب فعال برد با رله‌ها، ویگند و تنظیمات برد؛ ورودی/خروجی‌های درب در فهرست ورودی/خروجی‌های برد با شناسه همین درب هستند
    /// </summary>
    public class DtoGuardianControllerHardwareConfigDoor
    {
        /// <summary>
        /// شناسه درب در گاردین
        /// </summary>
        public Guid DoorId { get; set; }

        /// <summary>
        /// شماره درب روی برد
        /// </summary>
        public int DoorNumber { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// نوع درب (منطق برد برای هر نوع فرق دارد)
        /// </summary>
        public short DoorType { get; set; }

        /// <summary>
        /// همه رله‌های فعال درب
        /// </summary>
        public List<DtoGuardianControllerHardwareConfigRelay> Relays { get; set; } = [];

        /// <summary>
        /// مدت باز ماندن قفل پس از تردد مجاز
        /// </summary>
        public int UnlockDurationInSecond { get; set; }

        public int? DoorHeldTimeoutInSecond { get; set; }

        /// <summary>
        /// ویگند درب (درب با دوربین تشخیص چهره یا پلاک‌خوان ندارد)
        /// </summary>
        public Guid? WiegandId { get; set; }

        /// <summary>
        /// شماره فیزیکی ویگند روی برد
        /// </summary>
        public int? WiegandNumber { get; set; }

        /// <summary>
        /// جهت کارتخوان درب
        /// </summary>
        public short WiegandDirection { get; set; }

        public GuardianControllerWiegandDataTypeEnumeration? WiegandDataType { get; set; }

        public string WiegandTitle { get; set; }

        public bool MatchOnServerEnabled { get; set; }

        public bool MatchOnServerLocalCheckEnabled { get; set; }

        /// <summary>
        /// تقویم‌های حالت باز تقویمی
        /// </summary>
        public List<Guid> OpenCalendarIds { get; set; } = [];

        /// <summary>
        /// ردیف‌های حالت بسته تقویمی، هر کدام با تقویم و گروهی که در بازه‌های آن تردد دارد
        /// </summary>
        public List<DtoGuardianControllerHardwareConfigCloseCalendar> CloseCalendars { get; set; } = [];

        /// <summary>
        /// گروه‌های مدیران؛ فهرست خالی یعنی اجازه مدیر خاموش است
        /// </summary>
        public List<Guid> MasterAccessPrimerGroupIds { get; set; } = [];

        public GuardianControllerMasterPrimerResetPeriodEnumeration MasterAccessPrimerResetPeriod { get; set; }

        public short MasterAccessPrimerResetTime { get; set; }

        public bool MasterAccessPrimerApplyToEnterOnly { get; set; }

        public bool MasterAccessPrimerToggleMode { get; set; }
    }
}
