using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoDeviceInvalidAttendanceImage
    {
        public DateTime AttendanceDateTime { get; set; }
        public long UserIdInDevice { get; set; }
        public Guid DeviceId { get; set; }
        public byte[] Image { get; set; }
    }
}
