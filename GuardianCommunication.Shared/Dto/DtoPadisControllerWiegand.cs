using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoPadisControllerWiegand
    {
        public int Id { get; set; }
        public int DeviceNumber { get; set; }
        public string Title { get; set; }
        public int WiegandNumber { get; set; }
        public PadisControllerWiegandFormatEnumeration WiegandFormat { get; set; }
        public PadisControllerWiegandDataTypeEnumeration WiegandDataType { get; set; }
        public bool IsActive { get; set; }
    }
}
