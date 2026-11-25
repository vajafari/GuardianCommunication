using System;

namespace GuardianCommunication.Shared.SharedSettings
{
    public class GuardianConnectionStringConfig
    {
        public string GuardianDataBaseName { get; set; }
        public string GuardianLogDataBaseName { get; set; }
        public string SqlUsername { get; set; }
        public string SqlPassword { get; set; }
        public string ServerName { get; set; }
    }
}
