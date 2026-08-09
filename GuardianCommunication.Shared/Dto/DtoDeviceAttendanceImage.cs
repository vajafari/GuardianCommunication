using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoDeviceAttendanceImage
    {
        public DateTime AttendanceDateTime { get; set; }
        public long UserIdOnDevice { get; set; }
        public Guid DeviceId { get; set; }
        public byte[] Image { get; set; }
    }
}
