using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class OtherResourcesAttendanceModel
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public int? DeviceNumber { get; set; }
        [DataMember]
        public int? ReaderDeviceNumber { get; set; }
        [DataMember]
        public long EmployeeNumber { get; set; }
        [DataMember]
        public double AttendanceDateTime { get; set; }
        [DataMember]
        public int StatusCode { get; set; }
        [DataMember]
        public AttendanceSourceEnumeration AttendanceSource { get; set; }
        [DataMember]
        public bool IsSent { get; set; }
        [DataMember]
        public ModuleEnumeration ModuleId { get; set; }
        [DataMember]
        public bool CheckDuplicateInterval { get; set; }
        [DataMember]
        public DeviceIoTypeEnumeration IoType { get; set; }
        [DataMember]
        public int? CameraId { get; set; }
        [DataMember]
        public string RfCardNumber { get; set; }
        [DataMember]
        public int? VerificationStyle { get; set; }
        [DataMember]
        public int? DoorId { get; set; }
    }
}
