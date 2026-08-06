using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class UpdateFirmwareModel
	{
        [DataMember]
        public DeviceCommunicationModel DeviceInfo { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string FirmwareData { get; set; }

    }
}
