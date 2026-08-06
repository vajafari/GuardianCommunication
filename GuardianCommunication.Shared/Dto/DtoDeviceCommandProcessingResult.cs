using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoDeviceCommandProcessingResult
    {
        public int Id { get; set; }

        public DateTime CommandResponseTime { get; set; }

        public string CommandResponseResult { get; set; }

        public long? Mode { get; set; } = null;

    }
}
