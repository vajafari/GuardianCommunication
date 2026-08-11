using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoDeviceCommandProcessingDescription
    {
        public long NumericId { get; set; }

        public string Description { get; set; }

        public long? Mode { get; set; } = null;

    }
}
