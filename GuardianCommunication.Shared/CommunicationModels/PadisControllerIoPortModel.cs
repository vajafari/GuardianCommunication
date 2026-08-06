using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class PadisControllerIoPortModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int DeviceNumber { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public int IoNumber { get; set; }
        [DataMember]
        public bool IsActive { get; set; }

    }
}
