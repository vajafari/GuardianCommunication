using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// یک درب فعال برد برای ارسال پیکربندی سخت‌افزار
    /// </summary>
    [DataContract]
    public class GuardianControllerHardwareConfigDoorModel
    {
        /// <summary>
        /// شناسه درب در گاردین
        /// </summary>
        [DataMember]
        public Guid DoorId { get; set; }

        /// <summary>
        /// شماره درب روی برد
        /// </summary>
        [DataMember]
        public int DoorNumber { get; set; }

        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// نوع درب (منطق برد برای هر نوع فرق دارد)
        /// </summary>
        [DataMember]
        public short DoorType { get; set; }

        /// <summary>
        /// همه رله‌های فعال درب
        /// </summary>
        [DataMember]
        public List<GuardianControllerHardwareConfigRelayModel> Relays { get; set; } = [];

        /// <summary>
        /// مدت باز ماندن قفل پس از تردد مجاز
        /// </summary>
        [DataMember]
        public int UnlockDurationInSecond { get; set; }

        [DataMember]
        public int? DoorHeldTimeoutInSecond { get; set; }

        /// <summary>
        /// ویگند درب (درب با دوربین تشخیص چهره یا پلاک‌خوان ندارد)
        /// </summary>
        [DataMember]
        public Guid? WiegandId { get; set; }

        /// <summary>
        /// شماره فیزیکی ویگند روی برد
        /// </summary>
        [DataMember]
        public int? WiegandNumber { get; set; }

        /// <summary>
        /// جهت کارتخوان درب
        /// </summary>
        [DataMember]
        public short WiegandDirection { get; set; }

        [DataMember]
        public GuardianControllerWiegandDataTypeEnumeration? WiegandDataType { get; set; }

        [DataMember]
        public string WiegandTitle { get; set; }

        [DataMember]
        public bool MatchOnServerEnabled { get; set; }

        [DataMember]
        public bool MatchOnServerLocalCheckEnabled { get; set; }

        /// <summary>
        /// تقویم‌های حالت باز تقویمی
        /// </summary>
        [DataMember]
        public List<Guid> OpenCalendarIds { get; set; } = [];

        /// <summary>
        /// ردیف‌های حالت بسته تقویمی، هر کدام با تقویم و گروهی که در بازه‌های آن تردد دارد
        /// </summary>
        [DataMember]
        public List<GuardianControllerHardwareConfigCloseCalendarModel> CloseCalendars { get; set; } = [];

        /// <summary>
        /// گروه‌های مدیران؛ فهرست خالی یعنی اجازه مدیر خاموش است
        /// </summary>
        [DataMember]
        public List<Guid> MasterAccessPrimerGroupIds { get; set; } = [];

        [DataMember]
        public GuardianControllerMasterPrimerResetPeriodEnumeration MasterAccessPrimerResetPeriod { get; set; }

        [DataMember]
        public short MasterAccessPrimerResetTime { get; set; }

        [DataMember]
        public bool MasterAccessPrimerApplyToEnterOnly { get; set; }

        [DataMember]
        public bool MasterAccessPrimerToggleMode { get; set; }
    }
}
