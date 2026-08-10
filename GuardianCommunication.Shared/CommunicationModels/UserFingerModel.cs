using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class UserFingerModel
	{
		[DataMember]
		public long UserIdOnDevice { get; set; }
		[DataMember]
		public string TemplateData { get; set; }
		[DataMember]
		public int FingerIndex { get; set; }
		[DataMember]
		public uint CheckSum { get; set; }

	}
}
