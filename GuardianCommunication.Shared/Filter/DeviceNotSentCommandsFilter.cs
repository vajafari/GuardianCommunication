using GuardianCommunication.Shared.HardwareDefinition;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.Filter
{
    public class DeviceNotSentCommandsFilter
    {
        public ProducerEnumeration Producer { get; set; }
        public SdkVersionEnumeration SdkVersion { get; set; }
        public int Count { get; set; }
        public List<int> DeviceNumbers { get; set; }
        public List<string> DeviceSerialNumbers { get; set; }
    }
}
