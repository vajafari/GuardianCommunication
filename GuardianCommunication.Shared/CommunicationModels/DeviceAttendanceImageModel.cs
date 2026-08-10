using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class DeviceAttendanceImageModel
    {
        [DataMember]
        public double AttendanceDateTime { get; set; }
        [DataMember]
        public Guid DeviceId { get; set; }
        [DataMember]
        public long UserIdOnDevice { get; set; }
        [DataMember]
        public string Image { get; set; }
    }
}
