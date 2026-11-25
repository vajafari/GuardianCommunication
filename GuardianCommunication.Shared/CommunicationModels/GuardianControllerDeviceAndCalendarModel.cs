using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// ارسال تقویم به یک بورد کنترلر گاردین
    /// </summary>
    [DataContract]
    public class GuardianControllerDeviceAndCalendarModel
    {
        /// <summary>
        /// شناسه دستگاه
        /// </summary>
        [DataMember]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// تقویم
        /// </summary>
        [DataMember]
        public GuardianControllerCalendarModel Calendar { get; set; }
    }
}
