using System;
using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    /// <summary>
    /// ورودی/خروجی برد؛ درب آن اختیاری است
    /// </summary>
    [DataContract]
    public class GuardianControllerHardwareConfigIoPortModel
    {
        /// <summary>
        /// شناسه در گاردین؛ شناسه ثابت همین قلم روی برد
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// دربی که ورودی/خروجی به آن وصل است؛ خالی یعنی ورودی/خروجی کل برد
        /// </summary>
        [DataMember]
        public Guid? DoorId { get; set; }

        /// <summary>
        /// شماره فیزیکی ورودی/خروجی روی برد
        /// </summary>
        [DataMember]
        public int IoNumber { get; set; }

        [DataMember]
        public GuardianControllerGpioTypeEnumeration GpioType { get; set; }

        [DataMember]
        public GuardianControllerContactTypeEnumeration ContactType { get; set; }

        [DataMember]
        public string Title { get; set; }
    }
}
