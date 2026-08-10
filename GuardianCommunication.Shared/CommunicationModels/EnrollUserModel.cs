using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class EmployeeAndDeviceParamsModel
	{
		[DataMember]
		public List<UserAndDeviceModel> Records { get; set; }

	}
}
