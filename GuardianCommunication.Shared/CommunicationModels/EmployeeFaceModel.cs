using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class EmployeeFaceModel
    {
        [DataMember]
        public long EmployeeNumber { get; set; }
        [DataMember]
        public string TemplateData { get; set; }
        [DataMember]
        public int FaceIndex { get; set; }
        [DataMember]
        public uint CheckSum { get; set; }
        [DataMember]
        public int Length { get; set; }

        #region Suprema SDK 2 Face

        [DataMember]
        public byte? SupremaSdk2FaceFlag { get; set; }
        [DataMember]
        public int? SupremaSdk2FaceImageLen { get; set; }
        [DataMember]
        public string SupremaSdk2FaceImageData { get; set; }
        [DataMember]
        public byte? SupremaSdk2FaceNumOfTemplate { get; set; }

        #endregion

    }

}
