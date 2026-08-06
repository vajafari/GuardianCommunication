using GuardianCommunication.Hardware.PadisController.Definition;
using Newtonsoft.Json;

namespace GuardianCommunication.Hardware.PadisController.Model
{
    internal class PadisControllerErrorCommunicationModel
    {
        [JsonProperty("error_code")]
        public PadisControllerErrorEnumeration ErrorCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
