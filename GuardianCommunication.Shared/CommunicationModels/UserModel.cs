using System;
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
        public List<string> RfCardNumbers { get; set; } = [];
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
        public List<UserFingerModel> FingerDataList { get; set; } = [];
        [DataMember]
        public List<UserFaceModel> FaceDataList { get; set; } = [];
        [DataMember]
        public List<UserPalmModel> PalmDataList { get; set; } = [];
        [DataMember]
        public List<UserIrisModel> IrisDataList { get; set; } = [];
        [DataMember]
        public string ElevatorInfoInJsonFormat { get; set; }
        [DataMember]
        public string CabinetInfoInJsonFormat { get; set; }
        [DataMember]
        public DateTime StartDateTime { get; set; }
        [DataMember]
        public DateTime? EndDateTime { get; set; }
        [DataMember]
        public string VisibleLightImage { get; set; }
        [DataMember]
        public string HardwareProfileImage { get; set; }
        [DataMember]
        public DeviceUserTypeEnumeration UserType { get; set; }
        [DataMember]
        public bool StartAndEndHasTime { get; set; }




    }
}
