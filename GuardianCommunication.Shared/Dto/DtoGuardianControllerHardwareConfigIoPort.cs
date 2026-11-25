using System;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
    /// <summary>
    /// ورودی/خروجی برد؛ درب آن اختیاری است
    /// </summary>
    public class DtoGuardianControllerHardwareConfigIoPort
    {
        /// <summary>
        /// شناسه در گاردین؛ شناسه ثابت همین قلم روی برد
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// دربی که ورودی/خروجی به آن وصل است؛ خالی یعنی ورودی/خروجی کل برد
        /// </summary>
        public Guid? DoorId { get; set; }

        /// <summary>
        /// شماره فیزیکی ورودی/خروجی روی برد
        /// </summary>
        public int IoNumber { get; set; }

        public GuardianControllerGpioTypeEnumeration GpioType { get; set; }

        public GuardianControllerContactTypeEnumeration ContactType { get; set; }

        public string Title { get; set; }
    }
}
