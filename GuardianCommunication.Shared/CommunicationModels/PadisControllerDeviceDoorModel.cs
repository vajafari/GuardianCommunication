using System.Runtime.Serialization;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class PadisControllerDeviceDoorModel
	{
        [DataMember]
		public int Id { get; set; }
        [DataMember]
		public string Title { get; set; }
        [DataMember]
		public int DeviceNumber { get; set; }
        [DataMember]
		public string DeviceTitle { get; set; }
        [DataMember]
		public int DoorNumber { get; set; }
        [DataMember]
		public bool IsActive { get; set; }
        [DataMember]
		public int OpenDoorDelay { get; set; }
        [DataMember]
		public int? ReaderDeviceNumber { get; set; }
        [DataMember]
		public DeviceIoTypeEnumeration? ReaderIoType { get; set; }
        [DataMember]
		public int? WiegandId { get; set; }
        [DataMember]
		public int? PassVerificationIoPortId { get; set; }
        [DataMember]
		public int? OpenTimeCalendarNumber { get; set; }
        [DataMember]
		public string CombinationAccessGroupNumbersInJson { get; set; }
        [DataMember]
        public int DoorNumberOnDevice { get; set; }
	}
}
