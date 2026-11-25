using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// ارسال پیکربندی سخت‌افزار (درب‌های فعال با رله و ویگند، و ورودی/خروجی‌های فعال برد) به یک برد کنترلر گاردین
    /// </summary>
    [DataContract]
    public class GuardianControllerDeviceAndHardwareConfigModel
    {
        /// <summary>
        /// شناسه دستگاه
        /// </summary>
        [DataMember]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// درب‌های فعال برد
        /// </summary>
        [DataMember]
        public List<GuardianControllerHardwareConfigDoorModel> Doors { get; set; } = [];

        /// <summary>
        /// ورودی/خروجی‌های فعال برد، هر کدام با درب اختیاری خود
        /// </summary>
        [DataMember]
        public List<GuardianControllerHardwareConfigIoPortModel> IoPorts { get; set; } = [];
    }
}
