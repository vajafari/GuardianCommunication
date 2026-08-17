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

        public List<DtoUserFinger> FingerDataList { get; set; } = new List<DtoUserFinger>();

        public List<DtoUserFace> FaceDataList { get; set; } = new List<DtoUserFace>();

        public List<DtoUserPalm> PalmDataList { get; set; }

        public List<DtoUserIris> IrisDataList { get; set; }

        public string ElevatorInfoInJsonFormat { get; set; }

        public string CabinetInfoInJsonFormat { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public bool StartAndEndHasTime { get; set; }

        public byte[] VisibleLightImage { get; set; }

        public byte[] HardwareProfileImage { get; set; }

        public DeviceUserTypeEnumeration UserType { get; set; }

        public void ClearTemplateData()
        {
            FingerDataList = new List<DtoUserFinger>();
            FaceDataList = new List<DtoUserFace>();
            PalmDataList = new List<DtoUserPalm>();
            IrisDataList = new List<DtoUserIris>();
            VisibleLightImage = null;
        }

    }
}
