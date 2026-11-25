using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// ارسال یا حذف جداگانه دسترسی‌های آسان روی یک بورد کنترلر گاردین
    /// </summary>
    [DataContract]
    public class GuardianControllerDeviceAndEasyAccessesModel
    {
        /// <summary>
        /// شناسه دستگاه
        /// </summary>
        [DataMember]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// دسترسی‌های آسان
        /// </summary>
        [DataMember]
        public List<GuardianControllerEasyAccessModel> EasyAccesses { get; set; }
    }
}
