using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
	[DataContract]
    public class DeviceAttendanceImageModel
    {
        [DataMember]
        public double AttendanceDateTime { get; set; }
        [DataMember]
        public int DeviceNumber { get; set; }
        [DataMember]
        public long EmployeeNumber { get; set; }
        [DataMember]
        public string Image { get; set; }
    }
}
