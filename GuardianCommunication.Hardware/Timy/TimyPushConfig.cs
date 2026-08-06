namespace GuardianCommunication.Hardware.Timy
{
    public class TimyPushConfig
    {
        public int LongCommandTimeoutInSecond { get; set; }
        public int NormalCommandTimeoutInSecond { get; set; }
        public int GetCommandTimerIntervalInMillisecond { get; set; }
        public int WaitBetweenCommandSendInMilliseconds { get; set; }
    }
}
