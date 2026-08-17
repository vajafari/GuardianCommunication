using GuardianCommunication.Shared.Dto;

namespace GuardianCommunication.Hardware.Suprema.SupremaConcepts.V1
{
    public class SupremaSdk1ServerConfig
    {
        public int ServerPort { get; set; }
        public int MaxConnections { get; set; }
        public int StartDelayInSecond { get; set; }

        public DtoSystemConfigDeviceCommandSetting CommandSetting { get; set; }

    }
}
