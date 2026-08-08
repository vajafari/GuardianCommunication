using System;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoAttendance : DtoDatabaseEntityBase
    {
        public long PersonNumberOnDevice { get; set; }
        public DateTime AttendanceDateTime { get; set; }
        public Guid? DeviceId { get; set; }
        public Guid? CameraId { get; set; }
        public Guid? ReaderDeviceId { get; set; }
        public int? VerificationStyle { get; set; }
        public string RfCardNumber { get; set; }
        public int StatusCode { get; set; }
        public bool IsSentToGuardian { get; set; }
        public int SentToGuardianRetryCount { get; set; }
        public ModuleEnumeration ModuleId { get; set; }
        public DeviceIoTypeEnumeration IoType { get; set; }
        public AttendanceSourceEnumeration AttendanceSource { get; set; }
        public DeviceAttendanceIoRetrieveTypeEnumeration? DeviceAttendanceIoRetrieveType { get; set; }

        public int ApplicationIdNumber => (int)ModuleId;
        public short IoTypeNumber => (short)IoType;
        public int? DeviceAttendanceIoRetrieveTypeNumber => (int?)DeviceAttendanceIoRetrieveType;
        public int AttendanceSourceNumber => (int)AttendanceSource;

    }
}