using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class CameraModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string IPAddress { get; set; }
        [DataMember]
        public int Port { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public CameraTypeEnumeration CameraType { get; set; }
        [DataMember]
        public int Frame { get; set; }
        [DataMember]
        public int? DeviceNumber { get; set; }
        [DataMember]
        public string RtspString { get; set; }
        [DataMember]
        public CameraTaskEnumeration CameraTask { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public string ApiBasicAuthUsername { get; set; }
        [DataMember]
        public string ApiBasicAuthPassword { get; set; }
        [DataMember]
        public CameraSettingEnumeration CameraSettings { get; set; }
        [DataMember]
        public string CameraTaskSpecificSettingsInJson { get; set; }
        [DataMember]
        public ApplicationTypeEnumeration ApplicationId { get; set; }
        [DataMember]
        public DeviceIoTypeEnumeration IoType { get; set; }
        [DataMember]
        public int AreaNumber { get; set; }
        [DataMember]
        public string DeviceTitle { get; set; }
        [DataMember]
        public string AreaTitle { get; set; }

    }
}
