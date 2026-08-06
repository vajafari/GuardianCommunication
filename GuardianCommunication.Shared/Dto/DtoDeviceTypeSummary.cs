using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{
	public class DtoDeviceTypeSummary
	{
		public int DeviceTypeNumber { get; set; }
		public string Title { get; set; }
		public ProducerEnumeration ProducerNumber { get; set; }
		public SdkVersionEnumeration SdkVersion { get; set; }
		public short DeviceTypeCode { get; set; }
		public EnrollStandardEnumeration EnrollStandard { get; set; }
		public bool HasFingerPrint { get; set; }
		public bool HasFace { get; set; }
		public bool HasIris { get; set; }
		public bool HasRfReader { get; set; }
		public DoorTypeEnumeration DoorType { get; set; }
	}
}
