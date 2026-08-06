using System;

namespace GuardianCommunication.Shared.SharedSettings
{
    public class PadisConnectionStringConfig
    {
        public String KarnamaDataBaseName { get; set; }
        public String SQLUsername { get; set; }
        public String SQLPassword { get; set; }
        public String ServerName { get; set; }
        public String KarnamaLogDataBaseName { get; set; }
        public String PadisCommunicationDataBaseName { get; set; }
        public String ResultKarnama { get; set; }
        public String ResultKarnamaLog { get; set; }
        public String ResultPadisCommunication { get; set; }
    }
}
