using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class DeviceConnectionChangedModel
    {
	    [DataMember]
	    public bool IsConnected { get; set; }
	    [DataMember]
	    public int DeviceNumber { get; set; }
    }
}
