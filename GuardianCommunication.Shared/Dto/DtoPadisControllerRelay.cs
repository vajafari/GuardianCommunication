using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoPadisControllerRelay
    {
        public int Id { get; set; }
        public int DeviceNumber { get; set; }
        public string Title { get; set; }
        public int RelayNumber { get; set; }
        public GuardianControllerRelayTypeEnumeration RelayType { get; set; }
        public bool IsActive { get; set; }
    }
}