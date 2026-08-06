namespace GuardianCommunication.Hardware.Virdi
{
    public class VirdiServerConfig
    {
        public int ServerPort { get; set; }
        public int SyncOperationTimeout { get; set; }
        public int StartDelayInSecond { get; set; }
        public int MaxVisibleLightImageSizeInKb { get; set; }
        public int MaxVisibleLightImageSizeWidth { get; set; }
        public int MaxVisibleLightImageSizeHeight { get; set; }
        public DtoSystemConfigDeviceCommandSetting CommandSetting { get; set; }
    }
}
