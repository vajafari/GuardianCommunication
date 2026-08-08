using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoDeviceAttendanceImage
    {
        public DateTime AttendanceDateTime { get; set; }
        public long UserId { get; set; }
        public int DeviceNumber { get; set; }
        public byte[] Image { get; set; }
    }
}
