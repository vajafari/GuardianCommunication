using System.Runtime.Serialization;
using GuardianCommunication.Shared.CommunicationModels.Communication.Shared.CommunicationModels;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class NewUserEnrolledModel
	{
		[DataMember]
		public EmployeeModel EmployeeDeviceInfo { get; set; }

		[DataMember]
		public int DeviceNumber { get; set; }

        [DataMember]
        public EmployeeEnrolledSettingModel EmployeeEnrolledSetting { get; set; }

    }
}
