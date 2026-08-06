using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoDeviceUnauthorizedAttendanceImage
    {
        public long? EmployeeNumber { get; set; }
        public DateTime AttendanceDateTime { get; set; }
        public int DeviceNumber { get; set; }
        public byte[] Image { get; set; }
    }
}
