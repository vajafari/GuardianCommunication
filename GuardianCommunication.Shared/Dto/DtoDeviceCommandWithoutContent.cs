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

