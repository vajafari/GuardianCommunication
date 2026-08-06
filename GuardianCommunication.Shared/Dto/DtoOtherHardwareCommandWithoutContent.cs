using System;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoOtherHardwareCommandWithoutContent
    {
        public int Id { get; set; }

        public string HardwareSerialNumber { get; set; }

        public int? HardwareId { get; set; }

        public HardwareTypeEnumeration HardwareType { get; set; }

        public DateTime CommitTime { get; set; }

        public DateTime? SendTime { get; set; }

        public DateTime? ResponseTime { get; set; }

        public string ResponseValue { get; set; }

        public OtherHardwareCommandTypeEnumeration CommandType { get; set; }

        public CommandPriorityEnumeration Priority { get; set; }

        public long? ObjectId { get; set; }

        public int RetryCount { get; set; }

        public int MaxRetry { get; set; }

        public DateTime? Deadline { get; set; }

        public DateTime? VisiblilityTime { get; set; }

        public string Description { get; set; }

        public Guid? CommandIdentifier { get; set; }

    }
}
