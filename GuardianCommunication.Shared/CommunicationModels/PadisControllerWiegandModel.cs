using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class PadisControllerWiegandModel
    {
        [DataMember]
		public int Id { get; set; }
        [DataMember]
		public int DeviceNumber { get; set; }
        [DataMember]
		public string Title { get; set; }
        [DataMember]
		public int WiegandNumber { get; set; }
        [DataMember]
		public PadisControllerWiegandFormatEnumeration WiegandFormat { get; set; }
        [DataMember]
		public PadisControllerWiegandDataTypeEnumeration WiegandDataType { get; set; }
        [DataMember]
        public bool IsActive { get; set; }

	}
}
