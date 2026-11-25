using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// یک بازه از یک روز تقویم خاص
    /// </summary>
    [DataContract]
    public class GuardianControllerCalendarDetailsModel
    {
        /// <summary>
        /// شناسه در گاردین؛ شناسه ثابت همین قلم روی برد
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        /// <summary>
        /// ساعت شروع HHmm
        /// </summary>
        [DataMember]
        public short StartTime { get; set; }

        /// <summary>
        /// ساعت پایان HHmm (دقیقه پایان هم جزو بازه است)
        /// </summary>
        [DataMember]
        public short EndTime { get; set; }

        /// <summary>
        /// «مخصوص» بازه تقویم یا قالب فضا: 1 آقایان، 2 بانوان، 3 هر دو؛ خالی در تقویم و قالب عمومی
        /// </summary>
        [DataMember]
        public short? Audience { get; set; }

        /// <summary>
        /// «برای» بازه تقویم یا قالب فضا: 1 عضویت، 2 رزرو؛ خالی در تقویم و قالب عمومی
        /// </summary>
        [DataMember]
        public short? Purpose { get; set; }
    }
}
