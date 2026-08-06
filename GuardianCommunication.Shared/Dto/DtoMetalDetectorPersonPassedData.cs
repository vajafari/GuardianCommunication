using System;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoMetalDetectorPersonPassedData
    {
        public string DeviceId { get; set; }
        public DateTime Date { get; set; }
        public byte[] ZoneData { get; set; }
        public int TotalPassed { get; set; }
        public int TotalAlarm { get; set; }
    }
}
