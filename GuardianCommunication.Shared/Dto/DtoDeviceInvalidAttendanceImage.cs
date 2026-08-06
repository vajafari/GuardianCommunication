using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoDeviceInvalidAttendanceImage
    {
        public DateTime AttendanceDateTime { get; set; }
        public long EmployeeNumber { get; set; }
        public int DeviceNumber { get; set; }
        public byte[] Image { get; set; }
    }
}
