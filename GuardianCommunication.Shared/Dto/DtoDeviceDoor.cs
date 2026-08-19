using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoDeviceDoor : DtoDatabaseEntityBase
    {
        public Guid DeviceId { get; set; }
        public int DoorNumber { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public Guid? ReaderDeviceId { get; set; }
        public Guid? ReaderCameraId { get; set; }
    }
}
