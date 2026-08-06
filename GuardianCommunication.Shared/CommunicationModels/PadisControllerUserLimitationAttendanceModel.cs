using System.Runtime.Serialization;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class PadisControllerUserLimitationAttendanceModel
    {
        [DataMember]
        public double StartDateTime { get; set; }
        [DataMember]
        public double EndDateTime { get; set; }
        [DataMember]
        public int AttendanceCount { get; set; }
        [DataMember]
        public int? DoorId { get; set; }
        [DataMember]
        public DeviceIoTypeEnumeration? IoType { get; set; }
    }
}