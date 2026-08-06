using System.Collections.Generic;
using Newtonsoft.Json;

namespace GuardianCommunication.Hardware.Camera.PouyaFanavaran
{
    public class ReadAttendanceResponseModel
    {
        [JsonProperty("mainResult")]
        public string MainResult { get; set; }

        [JsonProperty("data")]
        public List<List<ReadAttendanceResponseItemModel>> Data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

    }
}
