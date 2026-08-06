using Newtonsoft.Json;

namespace GuardianCommunication.Hardware.Camera.PouyaFanavaran
{
    public class ReadAttendanceResponseItemModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("info")]
        public ReadAttendanceResponseItemInfoModel Info { get; set; }

    }
}
