using System;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoServerMatchData
    {
        public int DeviceNumber { get; set; }

        public DateTime EventDateTime { get; set; }

        public byte[] TemplateData { get; set; }

        public TemplateTypeEnumeration? TemplateType { get; set; }

        public ServerMatchingTypeEnumeration MatchType { get; set; }

        public string RfCardNumber { get; set; }

        public long? UserId { get; set; }

        public string Password { get; set; }

        public int? DoorId { get; set; }

    }
}
