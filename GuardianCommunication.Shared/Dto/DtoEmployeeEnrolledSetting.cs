namespace GuardianCommunication.Shared.Dto
{

    namespace Communication.Shared.CommunicationModels
    {
        public class DtoEmployeeEnrolledSetting
        {
            public bool OverwriteRfCardNumber { get; set; }
            public bool OverwriteDevicePassword { get; set; }
            public bool OverwritePrivilege { get; set; }
            public bool OverwriteVerificationStyle { get; set; }
            public bool OverwriteIsEnabled { get; set; }

            public static DtoEmployeeEnrolledSetting GetAllSettingInstance()
            {
                return new DtoEmployeeEnrolledSetting
                {
                    OverwriteRfCardNumber = true,
                    OverwriteDevicePassword = true,
                    OverwritePrivilege = true,
                    OverwriteVerificationStyle = true,
                    OverwriteIsEnabled = true,
                };
            }
        }
    }

}
