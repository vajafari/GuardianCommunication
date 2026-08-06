using System;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{

    public class DtoInvalidAttendance
    {
        public long Id { get; set; }
        public long? EmployeeNumber { get; set; }
        public string RfCardNumber { get; set; }
        public int? VerificationStyle { get; set; }
        public DateTime AttendanceDateTime { get; set; }
        public DeviceAttendanceIoRetrieveTypeEnumeration? DeviceAttendanceIoRetrieveType { get; set; }
        public AttendanceSourceEnumeration AttendanceSource { get; set; }
        public InvalidAttendanceReasonEnumeration Reason { get; set; }
        public int? DeviceNumber { get; set; }
        public int? DoorId { get; set; }
        public int StatusCode { get; set; }
        public byte[] Image { get; set; }

    }
}
