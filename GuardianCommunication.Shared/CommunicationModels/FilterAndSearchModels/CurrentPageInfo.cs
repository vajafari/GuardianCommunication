using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels.FilterAndSearchModels
{
	[DataContract]
	public class CurrentPageModel
	{
		[DataMember]
		public int PageNumber { get; set; }

		[DataMember]
		public int ItemPerPage { get; set; }

	}
}
