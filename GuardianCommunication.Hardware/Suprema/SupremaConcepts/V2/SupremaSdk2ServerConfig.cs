namespace GuardianCommunication.Hardware.Suprema.SupremaConcepts.V2
{
    public class SupremaSdk2ServerConfig
    {
        public int ServerPort { get; set; }
        public int ConnectionAliveTimer { get; set; }
        public int StartDelayInSecond { get; set; }
        public DtoSystemConfigDeviceCommandSetting CommandSetting { get; set; }
    }
}
