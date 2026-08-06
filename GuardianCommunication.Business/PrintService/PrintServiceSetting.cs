namespace GuardianCommunication.Business.PrintService
{
    public class PrintServiceSetting
    {
        public int SleepAfterPingCircleInMillisecond { get; set; }
        public int SleepWhenQueueIsEmptyInMillisecond { get; set; }
        public int PingTimeoutInSecond { get; set; }
        public int PrinterPerQueue { get; set; }
    }
}
