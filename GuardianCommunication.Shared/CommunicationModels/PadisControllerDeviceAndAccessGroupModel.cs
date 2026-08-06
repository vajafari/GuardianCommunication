using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class PadisControllerDeviceAndAccessGroupModel
    {
        [DataMember]
        public DeviceCommunicationModel DeviceInfo { get; set; }
        [DataMember]
        public PadisControllerAccessGroupModel AccessGroup { get; set; }
    }
}
