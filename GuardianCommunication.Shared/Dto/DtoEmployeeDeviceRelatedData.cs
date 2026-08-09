using System;
using System.Collections.Generic;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoUserDeviceRelatedData
    {
        public Guid Id { get; set; }

        public long UserIdOnDevice { get; set; }

        public List<string> RfCardNumbers { get; set; } = new List<string>();

        public string UserName { get; set; }

        public string Password { get; set; }

        public int Privilege { get; set; }

        public int VerificationStyle { get; set; }

        public bool IsEnable { get; set; }

        public List<DtoEmployeeFinger> FingerDataList { get; set; } = new List<DtoEmployeeFinger>();

        public List<DtoEmployeeFace> FaceDataList { get; set; } = new List<DtoEmployeeFace>();

        public List<DtoEmployeePalm> PalmDataList { get; set; }

        public List<DtoEmployeeIris> IrisDataList { get; set; }

        public DtoPadisControllerUserAccessData PadisControllerUserAccessData { get; set; }

        public List<int> TimeZones { get; set; } = new List<int>();

        public List<int> SupremaSdk1AccessGroups { get; set; } = new List<int>();

        public List<int> SupremaSdk2AccessGroups { get; set; } = new List<int>();

        public string VirdiAccessGroupCode { get; set; }
        
        public int? TimyWeekTimezoneDeviceIndex { get; set; }
        
        public string ElevatorInfoInJsonFormat { get; set; }

        public string CabinetInfoInJsonFormat { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public byte[] VisibleLightImage { get; set; }

        public byte[] HardwareProfileImage { get; set; }

        public DeviceUserTypeEnumeration UserType { get; set; }

        public void ClearTemplateData()
        {
            FingerDataList = new List<DtoEmployeeFinger>();
            FaceDataList = new List<DtoEmployeeFace>();
            PalmDataList = new List<DtoEmployeePalm>();
            IrisDataList = new List<DtoEmployeeIris>();
            VisibleLightImage = null;
        }

    }
}
