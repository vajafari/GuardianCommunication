using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class GetAttendanceModel
	{
		[DataMember]
		public DeviceCommunicationModel DeviceInfo { get; set; }

		[DataMember]
		public bool DeleteAttedance { get; set; }


	}
}
