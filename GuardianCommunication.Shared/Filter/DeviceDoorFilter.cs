using System;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.Filter
{
    public class DeviceDoorFilter
    {
        public List<Guid> Ids { get; set; }
        public List<Guid> DeviceIds { get; set; }
        public bool? IsActive { get; set; }
    }
}
