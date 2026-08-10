using System;
using System.Runtime.Serialization;
using GuardianCommunication.Shared.CommunicationModels.Communication.Shared.CommunicationModels;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class UserEnrolledModel
	{
		[DataMember]
		public UserModel UserDeviceInfo { get; set; }

		[DataMember]
		public Guid DeviceId { get; set; }

        [DataMember]
        public UserEnrolledSettingModel UserEnrolledSetting { get; set; }

    }
}
