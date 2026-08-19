using GuardianCommunication.Shared.Dto;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class UserFaceModel
    {
        [DataMember]
        public long UserId { get; set; }
        [DataMember]
        public string TemplateData { get; set; }
        [DataMember]
        public int FaceIndex { get; set; }
        [DataMember]
        public uint CheckSum { get; set; }
        [DataMember]
        public int Length { get; set; }

        public SupremaSdk2FaceTemplateAdditionalDataModel SupremaSdk2AdditionalData { get; set; }


    }

}
