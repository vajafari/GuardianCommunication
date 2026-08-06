using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class ListGuidModel
    {
        [DataMember]
        public List<Guid> Items { get; set; }
    }
}
