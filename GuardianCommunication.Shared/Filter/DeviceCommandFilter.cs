using System;
using System.Collections.Generic;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Filter
{
    public class DeviceCommandFilter
    {

        public List<long> Ids { get; set; }

        public List<Guid> CommandIdentifiers { get; set; }

        public List<long> EmployeeNumbers { get; set; }

        public long? EmployeeNumberLike { get; set; }

        public List<string> DeviceSerialNumbers { get; set; }

        public List<int> DeviceNumbers { get; set; }

        public DateTime? CommitTimeFrom { get; set; }

        public DateTime? CommitTimeTo { get; set; }

        public CommandPriorityEnumeration? Priority { get; set; }

        public DeviceCommandTypeEnumeration? CommandType { get; set; }

        public bool? IsSend { get; set; }

        public bool? VisiblilityTimeHasValue { get; set; }

        public DateTime? VisiblilityTimeFrom { get; set; }

        public DateTime? VisiblilityTimeTo { get; set; }

    }
}
