using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class PadisControllerDeviceAndAccessLevelModel
    {
        [DataMember]
        public DeviceCommunicationModel DeviceInfo { get; set; }
        [DataMember]
        public PadisControllerAccessLevelModel AccessLevel { get; set; }
    }
}
