using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class CameraImageModel
    {
        [DataMember]
        public string ImageBytes { get; set; }
        [DataMember]
        public string MimeType { get; set; }

    }
}
