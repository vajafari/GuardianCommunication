using System;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{

    public class DtoEmployeeAndDeviceParam
    {
        public Guid DeviceId { get; set; }
        public DtoUserDeviceRelatedData UserInfo { get; set; }
        public CommandPriorityEnumeration? CommandPriority { get; set; }
        public Guid? CommandIdentifier { get; set; }
    }

}
