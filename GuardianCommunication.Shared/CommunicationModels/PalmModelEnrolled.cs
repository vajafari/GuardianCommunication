using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class PalmModelEnrolled
    {
		[DataMember]
	    public UserPalmModel PalmInfo { get; set; }

	    [DataMember]
	    public int DeviceNumber { get; set; }
    }
}
