using System;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{

    public class DtoAttendance
    {
        public long Id { get; set; }
        public long EmployeeNumber { get; set; }
        public string RfCardNumber { get; set; }
        public DeviceIoTypeEnumeration IoType { get; set; }
        public int? VerificationStyle { get; set; }
        public DateTime AttendanceDateTime { get; set; }
        public DeviceAttendanceIoRetrieveTypeEnumeration? DeviceAttendanceIoRetrieveType { get; set; }
        public AttendanceSourceEnumeration AttendanceSource { get; set; }
        public int? DeviceNumber { get; set; }
        public int? ReaderDeviceNumber { get; set; }
        public int? DoorId { get; set; }
        public int? CameraId { get; set; }
        public int StatusCode { get; set; }
        public bool IsSent { get; set; }
        public bool IsInvalid { get; set; }
        public ApplicationTypeEnumeration ApplicationId { get; set; }
        public DateTime InsertDateTime { get; set; }

        public int ApplicationIdNumber => (int)ApplicationId;
        public short IoTypeNumber => (short)IoType;
        public int? DeviceAttendanceIoRetrieveTypeNumber => (int?)DeviceAttendanceIoRetrieveType;
        public int AttendanceSourceNumber => (int)AttendanceSource;

    }
}
