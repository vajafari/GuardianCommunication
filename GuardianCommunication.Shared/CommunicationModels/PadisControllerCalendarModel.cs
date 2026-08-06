using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class PadisControllerCalendarModel
    {
        [DataMember]
        public int CalendarNumber { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public List<PadisControllerCalendarDetailsModel> Details { get; set; }

    }
}
