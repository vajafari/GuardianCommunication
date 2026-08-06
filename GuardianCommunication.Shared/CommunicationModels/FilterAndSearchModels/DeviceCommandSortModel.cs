using System.Runtime.Serialization;
using GuardianCommunication.Shared.Filter;
using GuardianCommunication.Shared.SearchDataWrapper;

namespace GuardianCommunication.Shared.CommunicationModels.FilterAndSearchModels
{
	[DataContract]
	public class DeviceCommandSortModel
	{
		[DataMember]
		public DeviceCommandSortEnumeration SortEnum { get; set; }
		
		[DataMember]
		public SortTypeEnum SortType { get; set; }
		
	}
}
