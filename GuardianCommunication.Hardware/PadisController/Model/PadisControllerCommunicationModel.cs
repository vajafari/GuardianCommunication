using GuardianCommunication.Hardware.PadisController.Definition;

namespace GuardianCommunication.Hardware.PadisController.Model
{
    internal class PadisControllerCommunicationModel
    {

        public string DeviceSerialNumber { get; set; }

        public PadisControllerContentTypeEnumeration ContentType { get; set; }

        public object ContentInJsonFormat { get; set; }
    }
}
