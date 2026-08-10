using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class DeviceAndUserListModel
	{
		[DataMember]
		public Guid DeviceId { get; set; }
		[DataMember]
		public List<UserModel> EmployeeInfos { get; set; }
	}
}
