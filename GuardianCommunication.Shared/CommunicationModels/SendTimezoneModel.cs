using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class SendTimezoneModel
	{

		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }
		[DataMember]
		public TimezoneModel TimeZone { get; set; }
	}
}
