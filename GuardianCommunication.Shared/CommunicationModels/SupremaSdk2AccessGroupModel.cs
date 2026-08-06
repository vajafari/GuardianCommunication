using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class SupremaSdk2AccessGroupModel
    {
        [DataMember]
        public int AccessGroupNumber { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public List<int> AccessLevels { get; set; }
    }
}
