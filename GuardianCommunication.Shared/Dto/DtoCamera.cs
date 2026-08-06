using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoCamera
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public CameraTypeEnumeration CameraType { get; set; }
        public bool IsActive { get; set; }
        public CameraTaskEnumeration CameraTask { get; set; }
        public string RtspString { get; set; }
        public int? DeviceNumber { get; set; }
        public string ApiBasicAuthUsername { get; set; }
        public string ApiBasicAuthPassword { get; set; }
        public CameraSettingEnumeration CameraSettings { get; set; }
        public string CameraTaskSpecificSettingsInJson { get; set; }
        public ApplicationTypeEnumeration ApplicationId { get; set; }
        public DeviceIoTypeEnumeration IoType { get; set; }
        public int AreaNumber { get; set; }
    }

}
