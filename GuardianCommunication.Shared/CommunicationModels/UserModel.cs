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
        /// <summary>
        /// جنسیت کاربر: 1 آقا، 2 خانم؛ خالی برای کاربری که شخص نیست. فقط برد کنترلر گاردین آن را به کار می برد
        /// </summary>
        [DataMember]
        public short? Gender { get; set; }
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
        /// <summary>
        /// داده‌های دسترسی کاربر روی بورد کنترلر گاردین
        /// </summary>
        [DataMember]
        public GuardianControllerUserAccessDataModel GuardianControllerUserAccessData { get; set; }




    }
}
