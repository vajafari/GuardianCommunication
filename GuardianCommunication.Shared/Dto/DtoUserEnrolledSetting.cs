namespace GuardianCommunication.Shared.Dto
{

    namespace Communication.Shared.CommunicationModels
    {
        public class DtoUserEnrolledSetting
        {
            public bool OverwriteRfCardNumber { get; set; }
            public bool OverwriteDevicePassword { get; set; }
            public bool OverwritePrivilege { get; set; }
            public bool OverwriteVerificationStyle { get; set; }
            public bool OverwriteIsEnabled { get; set; }

            public static DtoUserEnrolledSetting GetAllSettingInstance()
            {
                return new DtoUserEnrolledSetting
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
