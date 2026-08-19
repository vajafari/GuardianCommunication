using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;
using System;
using System.Collections.Generic;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoApplicationEncodedConfig
    {
        public DateTime? ExpireDate { get; set; }

        public ModuleEnumeration Modules { get; set; }

        public ProducerEnumeration ActiveProducers { get; set; }

        public List<SdkVersionEnumeration> SupremaProducerVersions { get; set; }

    }

}
