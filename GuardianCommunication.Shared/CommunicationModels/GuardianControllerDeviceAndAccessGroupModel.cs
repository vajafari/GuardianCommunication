using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// ارسال گروه دسترسی به یک بورد کنترلر گاردین
    /// </summary>
    [DataContract]
    public class GuardianControllerDeviceAndAccessGroupModel
    {
        /// <summary>
        /// شناسه دستگاه
        /// </summary>
        [DataMember]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// گروه دسترسی
        /// </summary>
        [DataMember]
        public GuardianControllerAccessGroupModel AccessGroup { get; set; }
    }
}
