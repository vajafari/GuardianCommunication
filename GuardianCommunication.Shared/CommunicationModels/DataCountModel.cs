using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class DataCountModel
	{
		public int Count { get; set; }
		public int DeviceNumber { get; set; }
	}
}
