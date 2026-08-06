using System;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{

    public class DtoEmployeeAndDeviceParam
    {
        public DtoCommunicationDeviceData DeviceInfo { get; set; }
        public DtoEmployeeDeviceRelatedData UserInfo { get; set; }
        public CommandPriorityEnumeration? CommandPriority { get; set; }
        public Guid? CommandIdentifier { get; set; }
    }

}
