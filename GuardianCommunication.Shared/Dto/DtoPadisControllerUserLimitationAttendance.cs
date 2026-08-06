using System;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoPadisControllerUserLimitationAttendance
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int AttendanceCount { get; set; }
        public int? DoorId { get; set; }
        public DeviceIoTypeEnumeration? IoType { get; set; }
    }
}
