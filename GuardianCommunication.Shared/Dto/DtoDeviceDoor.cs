using System;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.Dto.Core;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
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
        public int OpenDoorDelay { get; set; }
        public string DeviceSpecificDoorSettingInJson { get; set; }
        public Guid? ReaderDeviceId { get; set; }
        public Guid? ReaderCameraId { get; set; }

        private DtoDeviceSpecificDoorSettingInJson _deviceSpecificDoorSetting;

        public DtoDeviceSpecificDoorSettingInJson DeviceSpecificDoorSetting
        {
            get
            {
                if (_deviceSpecificDoorSetting == null && DeviceSpecificDoorSettingInJson.IsNotNullOrEmpty())
                {
                    _deviceSpecificDoorSetting = ObjectHelper.DeserializeAsJson<DtoDeviceSpecificDoorSettingInJson>(DeviceSpecificDoorSettingInJson)
                                      ?? new DtoDeviceSpecificDoorSettingInJson();
                }

                return _deviceSpecificDoorSetting;
            }
            set => _deviceSpecificDoorSetting = value;
        }

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
