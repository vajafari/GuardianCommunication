using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class SupremaSdk2FaceTemplateAdditionalDataModel
    {
        [DataMember]
        public byte? SupremaSdk2FaceFlag { get; set; }
        [DataMember]
        public int? SupremaSdk2FaceImageLen { get; set; }
        [DataMember]
        public byte[] SupremaSdk2FaceImageData { get; set; }
        [DataMember]
        public byte? SupremaSdk2FaceNumOfTemplate { get; set; }
    }


}
