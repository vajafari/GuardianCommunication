using Newtonsoft.Json;

namespace GuardianCommunication.Hardware.Camera.PouyaFanavaran
{
    public class ReadAttendanceResponseItemInfoModel
    {
        [JsonProperty("plate")]
        public string Plate { get; set; }

        [JsonProperty("vio")]
        public string Vio { get; set; }

        [JsonProperty("finalSpeed")]
        public double FinalSpeed { get; set; }

        [JsonProperty("datetime")]
        public string Datetime { get; set; }

        [JsonProperty("epochtime")]
        public int Epochtime { get; set; }

        [JsonProperty("carType")]
        public string CarType { get; set; }

        [JsonProperty("direction")]
        public string Direction { get; set; }

        [JsonProperty("authority")]
        public string Authority { get; set; }

        [JsonProperty("passingLine")]
        public string PassingLine { get; set; }

        [JsonProperty("accuracy")]
        public string Accuracy { get; set; }

        [JsonProperty("distance")]
        public string Distance { get; set; }
    }
}
