using System.Collections.Generic;
using System.Runtime.Serialization;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.CommunicationModels
{
    [DataContract]
    public class EmployeeModel
    {
        [DataMember]
        public long EmployeeNumber { get; set; }
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
        public List<EmployeeFingerModel> FingerDataList { get; set; } = new List<EmployeeFingerModel>();
        [DataMember]
        public List<EmployeeFaceModel> FaceDataList { get; set; } = new List<EmployeeFaceModel>();
        [DataMember]
        public List<EmployeePalmModel> PalmDataList { get; set; } = new List<EmployeePalmModel>();
        [DataMember]
        public List<EmployeeIrisModel> IrisDataList { get; set; } = new List<EmployeeIrisModel>();
        [DataMember]
        public PadisControllerUserAccessDataModel PadisControllerUserAccessData { get; set; }
        [DataMember]
        public List<int> TimeZones { get; set; } = new List<int>();
        [DataMember]
        public List<int> SupremaSdk1AccessGroups { get; set; } = new List<int>();
        [DataMember]
        public List<int> SupremaSdk2AccessGroups { get; set; } = new List<int>();
        [DataMember]
        public string VirdiAccessGroupCode { get; set; }
        [DataMember]
        public int? TimyWeekTimezoneDeviceIndex { get; set; }
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
