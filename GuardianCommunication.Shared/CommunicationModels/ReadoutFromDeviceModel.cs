using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class ReadoutFromDeviceModel
	{
		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }
		[DataMember]
		public double StartDate { get; set; }
		[DataMember]
		public double EndDate { get; set; }
	}
}
