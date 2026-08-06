namespace GuardianCommunication.Shared.Dto
{
    public class DtoPwAutoCollectClearDataResult
    {
        public bool IsSuccessful { get; set; }
        public int OldRecordCount { get; set; }
        public int NewRecordCount { get; set; }
    }
}
