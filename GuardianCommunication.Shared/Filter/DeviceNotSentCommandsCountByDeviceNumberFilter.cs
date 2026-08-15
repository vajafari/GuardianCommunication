using System;
using GuardianCommunication.Shared.HardwareDefinition;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.Filter
{
    public class DeviceNotSentCommandsCountByDeviceNumberFilter
    {
        public ProducerEnumeration Producer { get; set; }
        public SdkVersionEnumeration SdkVersion { get; set; }
        public List<int> DeviceNumbers { get; set; }
    }
}
