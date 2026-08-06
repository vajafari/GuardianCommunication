using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class PadisControllerRelayModel
    {

        [DataMember]
		public int Id { get; set; }
        [DataMember]
		public int DeviceNumber { get; set; }
        [DataMember]
		public string Title { get; set; }
        [DataMember]
		public int RelayNumber { get; set; }
        [DataMember]
		public PadisControllerRelayTypeEnumeration RelayType { get; set; }
        [DataMember]
        public bool IsActive { get; set; }

	}
}
