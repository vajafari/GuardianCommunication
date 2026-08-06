using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class PadisControllerEasyPermissionModel
    {
        [DataMember]
        public double StartDateTime { get; set; }
        [DataMember]
        public double EndDateTime { get; set; }
        [DataMember]
        public int DoorId { get; set; }
        [DataMember]
        public int? CalendarNumber { get; set; }
    }
}
