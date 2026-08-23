using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;
using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoDevice : DtoDatabaseEntityBase
    {
        public int DeviceNumber { get; set; }
        public Guid DeviceTypeId { get; set; }
        public Guid LocationId { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public string SerialNumber { get; set; }
        public string DevicePassword { get; set; }
        public DeviceConnectionModeEnumeration ConnectionMode { get; set; }
        public ConnectionTypeEnumeration ConnectionType { get; set; }
        public string DeviceIp { get; set; }
        public string DeviceSettingInJson { get; set; }
        public int? TcpPort { get; set; }
        public int ConnectTimeout { get; set; }
        public ModuleEnumeration ModuleId { get; set; }
        public DeviceIoTypeEnumeration IoType { get; set; }
        public int DeviceTypeNumber { get; set; }
        public ProducerEnumeration ProducerNumber { get; set; }
        public SdkVersionEnumeration SdkVersion { get; set; }
        public short DeviceTypeCode { get; set; }
        public EnrollStandardEnumeration EnrollStandard { get; set; }
        public bool HasFingerPrint { get; set; }
        public bool HasFace { get; set; }
        public bool HasVisiblelight { get; set; }
        public bool HasRfReader { get; set; }
        public bool HasPalm { get; set; }
        public bool HasIris { get; set; }
        public int LocationNumber { get; set; }
        public string DeviceDescription { get; set; }
        public string IanaTimeZoneId { get; set; }


        private DtoDeviceSettings _deviceSettings;
        public DtoDeviceSettings DeviceSettings
        {
            get
            {
                if (_deviceSettings == null && DeviceSettingInJson.IsNotNullOrEmpty())
                {
                    _deviceSettings = ObjectHelper.DeserializeAsJson<DtoDeviceSettings>(DeviceSettingInJson)
                                      ?? new DtoDeviceSettings();
                }
                return _deviceSettings;
            }
            set => _deviceSettings = value;
        }


    }
}
