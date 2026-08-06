using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class PadisControllerAccessGroupModel
    {
        [DataMember]
        public int AccessGroupNumber { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public List<PadisControllerAccessGroupAccessDataModel> AccessData { get; set; }

    }
}
