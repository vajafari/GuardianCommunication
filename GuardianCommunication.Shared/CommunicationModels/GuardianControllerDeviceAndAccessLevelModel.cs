using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// ارسال سطح دسترسی به یک بورد کنترلر گاردین
    /// </summary>
    [DataContract]
    public class GuardianControllerDeviceAndAccessLevelModel
    {
        /// <summary>
        /// شناسه دستگاه
        /// </summary>
        [DataMember]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// سطح دسترسی
        /// </summary>
        [DataMember]
        public GuardianControllerAccessLevelModel AccessLevel { get; set; }
    }
}
