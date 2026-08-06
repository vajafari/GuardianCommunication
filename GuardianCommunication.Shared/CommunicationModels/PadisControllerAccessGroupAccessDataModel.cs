using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class PadisControllerAccessGroupAccessDataModel
    {
        [DataMember]
        public int AccessGroupNumber { get; set; }
        [DataMember]
        public int AccessLevelNumber { get; set; }
        [DataMember]
        public int CalendarNumber { get; set; }
    }
}
