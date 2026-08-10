using System;
using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class OtherResourcesAttendanceModel
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public Guid? DeviceId { get; set; }
        [DataMember]
        public Guid? ReaderDeviceId { get; set; }
        [DataMember]
        public long UserIdOnDevice { get; set; }
        [DataMember]
        public double AttendanceDateTime { get; set; }
        [DataMember]
        public int StatusCode { get; set; }
        [DataMember]
        public AttendanceSourceEnumeration AttendanceSource { get; set; }
        [DataMember]
        public bool IsSentToGuardian { get; set; }
        [DataMember]
        public ModuleEnumeration ModuleId { get; set; }
        [DataMember]
        public bool CheckDuplicateInterval { get; set; }
        [DataMember]
        public DeviceIoTypeEnumeration IoType { get; set; }
        [DataMember]
        public Guid? CameraId { get; set; }
        [DataMember]
        public string RfCardNumber { get; set; }
        [DataMember]
        public int? VerificationStyle { get; set; }
        [DataMember]
        public Guid? DoorId { get; set; }
    }
}
