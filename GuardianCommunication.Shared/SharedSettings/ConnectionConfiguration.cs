namespace GuardianCommunication.Shared.SharedSettings
{
	public class ConnectionConfiguration
	{
        public string ConnectionString { get; set; }
        public string LogConnectionString { get; set; }
        public int CommandTimeout { get; set; }
        public int LongCommandTimeout { get; set; }
    }
}
