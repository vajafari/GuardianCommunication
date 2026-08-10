using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;
using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoDeviceDoor
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Guid DeviceId { get; set; }
        public int DoorNumber { get; set; }
        public bool IsActive { get; set; }
        public Guid? ReaderDeviceId { get; set; }
        public Guid? ReaderCameraId { get; set; }
    }
}
