using System;
using System.Runtime.Serialization;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class DeviceInvalidAttendanceModel
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public long? UserIdOnDevice { get; set; }
        [DataMember]
        public int VerificationStyle { get; set; }
        [DataMember]
        public double AttendanceDateTime { get; set; }
        [DataMember]
        public AttendanceSourceEnumeration AttendanceSource { get; set; }
        [DataMember]
        public Guid DeviceId { get; set; }
        [DataMember]
        public int StatusCode { get; set; }
        [DataMember]
        public string RfCardNumber { get; set; }
        [DataMember]
        public InvalidAttendanceReasonEnumeration Reason { get; set; }
        [DataMember]
        public string Image { get; set; }
        [DataMember]
        public Guid? DoorId { get; set; }
    }
}
