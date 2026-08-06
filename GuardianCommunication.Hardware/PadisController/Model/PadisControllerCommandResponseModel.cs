namespace GuardianCommunication.Hardware.PadisController.Model
{
    public class PadisControllerCommandResponseModel
    {
        public int CommandId { get; set; }
        public bool IsSuccessFull { get; set; }
        public string ProcessResultText { get; set; }
    }
}
