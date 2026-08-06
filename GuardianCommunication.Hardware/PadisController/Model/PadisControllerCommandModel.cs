namespace GuardianCommunication.Hardware.PadisController.Model
{
    internal class PadisControllerCommandModel
    {
        public int CommandId { get; set; }

        public DeviceCommandTypeEnumeration Command { get; set; }

        public object CommandContent { get; set; }
    }
}
