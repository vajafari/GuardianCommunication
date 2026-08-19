using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class UserPalmModelEnrolled
    {
		[DataMember]
	    public UserPalmModel PalmInfo { get; set; }

	    [DataMember]
	    public Guid DeviceId { get; set; }
    }
}
