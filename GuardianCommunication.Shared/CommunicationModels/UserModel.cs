using System.Collections.Generic;
using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class UserModel
    {
        [DataMember]
        public long UserIdOnDevice { get; set; }
        [DataMember]
        public List<string> RfCardNumbers { get; set; } = new List<string>();
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public int Privilege { get; set; }
        [DataMember]
        public int VerificationStyle { get; set; }
        [DataMember]
        public bool IsEnable { get; set; }
        [DataMember]
        public List<UserFingerModel> FingerDataList { get; set; } = new List<UserFingerModel>();
        [DataMember]
        public List<UserFaceModel> FaceDataList { get; set; } = new List<UserFaceModel>();
        [DataMember]
        public List<UserPalmModel> PalmDataList { get; set; } = new List<UserPalmModel>();
        [DataMember]
        public List<UserIrisModel> IrisDataList { get; set; } = new List<UserIrisModel>();
        [DataMember]
        public string ElevatorInfoInJsonFormat { get; set; }
        [DataMember]
        public string CabinetInfoInJsonFormat { get; set; }
        [DataMember]
        public double StartDate { get; set; }
        [DataMember]
        public double? EndDate { get; set; }
        [DataMember]
        public string VisibleLightImage { get; set; }
        [DataMember]
        public string HardwareProfileImage { get; set; }
        [DataMember]
        public DeviceUserTypeEnumeration UserType { get; set; }


    }
}
