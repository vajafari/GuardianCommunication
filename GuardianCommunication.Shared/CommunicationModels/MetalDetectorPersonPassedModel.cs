namespace GuardianCommunication.Shared.CommunicationModels
{
    public class MetalDetectorPersonPassedModel
	{
		public string DeviceId { get; set; }
		public double PassingDateTime { get; set; }
		public string ZoneData { get; set; }
        public int TotalPassed { get; set; }
        public int TotalAlarm { get; set; }
    }
}
