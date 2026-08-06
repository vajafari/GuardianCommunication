using System.Runtime.Serialization;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class DeviceTypeSummaryModel
	{
		[DataMember]
		public int DeviceTypeNumber { get; set; }
		[DataMember]
		public string Title { get; set; }
		[DataMember]
		public ProducerEnumeration ProducerNumber { get; set; }
		[DataMember]
		public SdkVersionEnumeration SdkVersion { get; set; }
		[DataMember]
		public short DeviceTypeCode { get; set; }
		[DataMember]
		public EnrollStandardEnumeration EnrollStandard { get; set; }
		[DataMember]
		public bool HasFingerPrint { get; set; }
		[DataMember]
		public bool HasFace { get; set; }
        [DataMember]
        public bool HasIris { get; set; }
		[DataMember]
		public bool HasRfReader { get; set; }
		[DataMember]
		public DoorTypeEnumeration DoorType { get; set; }
	}
}
