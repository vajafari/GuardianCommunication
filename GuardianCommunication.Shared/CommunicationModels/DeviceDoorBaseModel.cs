using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
	public class DeviceDoorBaseModel
	{
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public Guid DeviceId { get; set; }

        [DataMember]
        public int DoorNumber { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public int OpenDoorDelay { get; set; }

        [DataMember]
        public Guid? ReaderDeviceId { get; set; }

        [DataMember]
        public Guid? ReaderCameraId { get; set; }

    }
}
