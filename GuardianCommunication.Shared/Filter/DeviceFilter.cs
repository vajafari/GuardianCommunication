using System;
using System.Collections.Generic;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Filter
{
    public class DeviceFilter
    {
        public List<Guid> Ids { get; set; }
        public List<int> DeviceNumbers { get; set; }
        public List<Guid> LocationIds { get; set; }
        public List<Guid> DeviceTypeIds { get; set; }
        public List<DeviceIoTypeEnumeration> IoTypes { get; set; }
        public List<ProducerEnumeration> Producers { get; set; }
        public List<SdkVersionEnumeration> SdkVersions { get; set; }
        public List<short> ConnectionModes { get; set; }
        public List<string> SerialNumbers { get; set; }
        public bool? IsActive { get; set; }

        public List<short> IoTypeValues => IoTypes.IsCollectionNotNullOrEmpty()
            ? IoTypes.ConvertAll(row => (short)row)
            : null;

        public List<short> ProducerValues => Producers.IsCollectionNotNullOrEmpty()
            ? Producers.ConvertAll(row => (short)row)
            : null;

        public List<short> SdkVersionValues => SdkVersions.IsCollectionNotNullOrEmpty()
            ? SdkVersions.ConvertAll(row => (short)row)
            : null;
    }
}
