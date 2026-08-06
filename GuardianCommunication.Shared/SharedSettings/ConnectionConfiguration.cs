namespace GuardianCommunication.Shared.SharedSettings
{
	public class ConnectionConfiguration
	{
		public string ConnectionString { get; set; }
        public string KarnamaLogConnectionString { get; set; }
		public int Timeout { get; set; }
		public int LongTimeout { get; set; }
	}
}
