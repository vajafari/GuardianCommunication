namespace GuardianCommunication.Hardware.XRayDevice
{
    public class XReaDeviceServerConfig
    {
        public int IntervalToRetrySendInSecond { get; set; }
        public int SleepAfterNoFileInMilliSecond { get; set; }
        public int WaitBeforeAddToQueueInMilliSecond { get; set; }
    }
}
