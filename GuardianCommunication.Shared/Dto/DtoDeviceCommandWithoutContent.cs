using System;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoDeviceCommandWithoutContent : DtoDatabaseEntityBase
    {

        public Guid DeviceId { get; set; }

        public int DeviceNumber { get; set; }

        public string DeviceSerialNumber { get; set; }
        
        public long? UserIdOnDevice { get; set; }

        public DateTime CommitTime { get; set; }

        public DateTime? SendTime { get; set; }

        public DateTime? ResponseTime { get; set; }

        public string ResponseValue { get; set; }

        public DeviceCommandTypeEnumeration CommandType { get; set; }

        public int RetryCount { get; set; }

        public CommandPriorityEnumeration Priority { get; set; }
        
        public int MaxRetry { get; set; }

        public DateTime? Deadline { get; set; }

        public ProducerEnumeration ProducerNumber { get; set; }

        public SdkVersionEnumeration SdkVersion { get; set; }

        public DateTime? VisiblilityTime { get; set; }

        public string Description { get; set; }

        public Guid? CommandIdentifier { get; set; }

    }
}


Id uniqueidentifier	Unchecked
    	bigint	Unchecked
AttendanceDateTime	datetime2(7)	Unchecked
    DeviceId	uniqueidentifier	Checked
CameraId	uniqueidentifier	Checked
    ReaderDeviceId	uniqueidentifier	Checked
VerificationStyle	int	Checked
RfCardNumber	nvarchar(100)	Checked
    StatusCode	int	Unchecked
IsSentToGuardian	bit	Unchecked
    SentToGuardianRetryCount	int	Unchecked
ModuleId	int	Checked
IoType	smallint	Unchecked
    AttendanceSource	smallint	Unchecked
DeviceAttendanceIoRetrieveType	smallint	Checked
    InsertedAt	datetime2(7)	Unchecked
    UpdatedAt	datetime2(7)	Checked