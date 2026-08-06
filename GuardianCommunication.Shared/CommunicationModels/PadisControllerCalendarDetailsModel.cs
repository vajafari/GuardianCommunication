using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class PadisControllerCalendarDetailsModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int PadisControllerCalendarNumber { get; set; }
        [DataMember]
        public double Date { get; set; }
        [DataMember]
        public short StartTime { get; set; }
        [DataMember]
        public short EndTime { get; set; }
    }
}
