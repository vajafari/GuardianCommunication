using System;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    public class MatchOnServerDeviceAttendanceModel
    {
        public long Id { get; set; }
        public long UserIdOnDevice { get; set; }
        public int VerificationStyle { get; set; }
        public double AttendanceDateTime { get; set; }
        public AttendanceSourceEnumeration AttendanceSource { get; set; }
        public Guid DeviceId { get; set; }
        public int StatusCode { get; set; }
        public string RfCardNumber { get; set; }
        public DeviceIoTypeEnumeration IoType { get; set; }
        public ModuleEnumeration ModuleId { get; set; }
    }
}
