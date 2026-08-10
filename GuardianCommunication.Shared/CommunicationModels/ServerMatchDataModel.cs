using System;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    public class ServerMatchDataModel
    {
        public Guid DeviceId { get; set; }

        public double EventDateTime { get; set; }

        public string TemplateData { get; set; }

        public TemplateTypeEnumeration? TemplateType { get; set; }

        public ServerMatchingTypeEnumeration MatchType { get; set; }

        public string RfCardNumber { get; set; }

        public long? UserIdOnDevice { get; set; }

        public string Password { get; set; }

        public int? DoorId { get; set; }

    }
}
