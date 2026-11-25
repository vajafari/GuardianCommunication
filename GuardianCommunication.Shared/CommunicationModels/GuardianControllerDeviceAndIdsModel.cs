using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// حذف قالب‌های تقویم، تقویم‌ها، سطح‌ها یا گروه‌های دسترسی از یک برد کنترلر گاردین با شناسه گاردین
    /// </summary>
    [DataContract]
    public class GuardianControllerDeviceAndIdsModel
    {
        /// <summary>
        /// شناسه دستگاه
        /// </summary>
        [DataMember]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// شناسه‌های گاردین
        /// </summary>
        [DataMember]
        public List<Guid> Ids { get; set; }
    }
}
