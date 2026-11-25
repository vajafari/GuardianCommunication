using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// ارسال قالب تقویم به یک بورد کنترلر گاردین
    /// </summary>
    [DataContract]
    public class GuardianControllerDeviceAndCalendarTemplateModel
    {
        /// <summary>
        /// شناسه دستگاه
        /// </summary>
        [DataMember]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// قالب تقویم
        /// </summary>
        [DataMember]
        public GuardianControllerCalendarTemplateModel CalendarTemplate { get; set; }
    }
}
