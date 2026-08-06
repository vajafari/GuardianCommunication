using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class SupremaSdk1DeviceAndDoorWithDelayModel
    {
        [DataMember]
        public DeviceCommunicationModel DeviceInfo { get; set; }
        [DataMember]
        public SupremaSdk1DeviceDoorModel Door { get; set; }
        [DataMember]
        public int DelayInSecond { get; set; }
    }
}
