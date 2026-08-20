using System;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoDeviceDoorFullInfo : DtoDatabaseEntityBase
    {
        public Guid DeviceId { get; set; }
        public int DoorNumber { get; set; }
        public int DoorIdOnDevice { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public Guid? ReaderDeviceId { get; set; }
        public Guid? ReaderCameraId { get; set; }

        // Main device (the door's own device — drives security via LocationId)
        public ProducerEnumeration ProducerNumber { get; set; }
        public SdkVersionEnumeration SdkVersion { get; set; }
        public ModuleEnumeration ModuleId { get; set; }
        public Guid LocationId { get; set; }

        // Reader (resolved through ReaderDeviceId XOR ReaderCameraId)
        public ModuleEnumeration ReaderModuleId { get; set; }
        public DeviceIoTypeEnumeration ReaderIoType { get; set; }
        public Guid ReaderLocationId { get; set; }

        /// <summary>
        /// فقط زمانی که ReaderDeviceId مقدار دارد؛ در غیر این صورت NULL
        /// </summary>
        public ProducerEnumeration? ReaderProducerNumber { get; set; }

        /// <summary>
        /// فقط زمانی که ReaderDeviceId مقدار دارد؛ در غیر این صورت NULL
        /// </summary>
        public SdkVersionEnumeration? ReaderSdkVersion { get; set; }

    }
}
