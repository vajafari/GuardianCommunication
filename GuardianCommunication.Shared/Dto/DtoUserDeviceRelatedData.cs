using System;
using System.Collections.Generic;
using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Shared.Dto
{
    public class DtoUserDeviceRelatedData
    {
        public Guid Id { get; set; }

        public long UserIdOnDevice { get; set; }

        public List<string> RfCardNumbers { get; set; } = [];

        public string UserName { get; set; }

        public string Password { get; set; }

        public int Privilege { get; set; }

        public int VerificationStyle { get; set; }

        public bool IsEnable { get; set; }

        public List<DtoUserFinger> FingerDataList { get; set; } = [];

        public List<DtoUserFace> FaceDataList { get; set; } = [];

        public List<DtoUserPalm> PalmDataList { get; set; }

        public List<DtoUserIris> IrisDataList { get; set; }

        public string ElevatorInfoInJsonFormat { get; set; }

        public string CabinetInfoInJsonFormat { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public bool StartAndEndHasTime { get; set; }

        public byte[] VisibleLightImage { get; set; }

        public byte[] HardwareProfileImage { get; set; }

        public DeviceUserTypeEnumeration UserType { get; set; }

        public void ClearTemplateData()
        {
            FingerDataList = [];
            FaceDataList = [];
            PalmDataList = [];
            IrisDataList = [];
            VisibleLightImage = null;
        }

    }
}
