namespace GuardianCommunication.Shared.Dto
{
    public class DtoOtherHardwareCommand : DtoOtherHardwareCommandWithoutContent
    {
        public string CommandContent { get; set; }

        public string HardwareContent { get; set; }
    }
}
