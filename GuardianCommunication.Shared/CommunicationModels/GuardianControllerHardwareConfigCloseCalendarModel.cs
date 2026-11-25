using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// ردیف حالت بسته تقویمی درب: در بازه‌های تقویم درب قفل است و فقط اعضای گروه با کارت رد می‌شوند
    /// </summary>
    [DataContract]
    public class GuardianControllerHardwareConfigCloseCalendarModel
    {
        /// <summary>
        /// شناسه ردیف در گاردین
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Guid CalendarId { get; set; }

        [DataMember]
        public Guid AccessGroupId { get; set; }
    }
}
