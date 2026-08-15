using System;
using GuardianCommunication.Shared.HardwareDefinition;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.Filter
{
    public class DeviceNotSentCommandsCountByDeviceIdFilter
    {
        public ProducerEnumeration Producer { get; set; }
        public SdkVersionEnumeration SdkVersion { get; set; }
        public List<Guid> DeviceIds { get; set; }
    }
}
