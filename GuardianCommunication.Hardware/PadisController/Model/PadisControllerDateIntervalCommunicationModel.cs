using System.Text.Json.Serialization;

namespace GuardianCommunication.Hardware.PadisController.Model
{
    public class PadisControllerDateIntervalCommunicationModel
    {
        [JsonPropertyName("start_time_ms")]
        public long? StartTime { get; set; }

        [JsonPropertyName("end_time_ms")]
        public long? EndTime { get; set; }

        [JsonPropertyName("limit")]
        public long? Limit { get; set; }

        [JsonPropertyName("offset")]
        public long? Offset { get; set; }
    }
}
