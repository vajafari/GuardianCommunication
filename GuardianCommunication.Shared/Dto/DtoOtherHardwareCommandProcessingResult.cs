using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoOtherHardwareCommandProcessingResult
    {
        public int Id { get; set; }

        public DateTime CommandResponseTime { get; set; }

        public DateTime? CommandSendTime { get; set; }

        public string CommandResponseResult { get; set; }

    }
}
