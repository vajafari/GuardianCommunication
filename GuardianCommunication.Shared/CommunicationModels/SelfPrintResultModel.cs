namespace GuardianCommunication.Shared.CommunicationModels
{
    public class SelfPrintResultModel
    {
        public long Id { get; set; }
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public string AdditionalData { get; set; }
    }
}
