using System;
using System.Collections.Generic;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Filter
{
    public class OtherHardwareCommandFilter
    {

        public List<long> Ids { get; set; }

        public List<Guid> CommandIdentifiers { get; set; }

        public List<long> EmployeeNumbers { get; set; }

        public long? EmployeeNumberLike { get; set; }

        public List<string> CameraSerialNumbers { get; set; }

        public List<int> CameraIds { get; set; }

        public DateTime? CommitTimeFrom { get; set; }

        public DateTime? CommitTimeTo { get; set; }

        public CommandPriorityEnumeration? Priority { get; set; }

        public OtherHardwareCommandTypeEnumeration? CommandType { get; set; }

        public bool? IsSend { get; set; }

        public bool? VisiblilityTimeHasValue { get; set; }

        public DateTime? VisiblilityTimeFrom { get; set; }

        public DateTime? VisiblilityTimeTo { get; set; }

    }
}
