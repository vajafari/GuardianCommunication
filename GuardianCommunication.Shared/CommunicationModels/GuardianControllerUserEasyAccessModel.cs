using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// دسترسی آسان کاربر روی برد (همراه اطلاعات کاربر)
    /// </summary>
    [DataContract]
    public class GuardianControllerUserEasyAccessModel
    {
        /// <summary>
        /// شناسه دسترسی آسان
        /// </summary>
        [DataMember]
        public Guid EasyAccessId { get; set; }

        [DataMember]
        public Guid AccessLevelId { get; set; }

        [DataMember]
        public Guid? CalendarId { get; set; }

        /// <summary>
        /// شروع اعتبار (زمان محلی)
        /// </summary>
        [DataMember]
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// پایان اعتبار (زمان محلی)؛ خالی یعنی بدون پایان (دسترسی آسانی که فقط سقف تردد دارد)
        /// </summary>
        [DataMember]
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// سقف تعداد تردد
        /// </summary>
        [DataMember]
        public int? AttendanceCount { get; set; }
    }
}
