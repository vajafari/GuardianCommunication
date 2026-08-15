using System;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{

    public class DtoInvalidAttendance
    {
        public long Id { get; set; }
        public long? UserIdOnDevice { get; set; }
        public string RfCardNumber { get; set; }
        public int? VerificationStyle { get; set; }
        public DateTime AttendanceDateTime { get; set; }
        public DeviceAttendanceIoRetrieveTypeEnumeration? DeviceAttendanceIoRetrieveType { get; set; }
        public AttendanceSourceEnumeration AttendanceSource { get; set; }
        public InvalidAttendanceReasonEnumeration Reason { get; set; }
        public Guid? DeviceId { get; set; }
        public Guid? DoorId { get; set; }
        public int StatusCode { get; set; }
        public byte[] Image { get; set; }

    }
}
