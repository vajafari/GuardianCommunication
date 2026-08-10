using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoDeviceUnauthorizedAttendanceImage
    {
        public long? UserIdInDevice { get; set; }
        public DateTime AttendanceDateTime { get; set; }
        public Guid DeviceId { get; set; }
        public byte[] Image { get; set; }
    }
}
