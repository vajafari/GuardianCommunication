using System;
using GuardianCommunication.Shared.HardwareDefinition;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.Filter
{
    public class DeviceNotSentCommandsCountByDeviceSerialNumberFilter
    {
        public ProducerEnumeration Producer { get; set; }
        public SdkVersionEnumeration SdkVersion { get; set; }
        public List<string> DeviceSerialNumbers { get; set; }
    }
}
