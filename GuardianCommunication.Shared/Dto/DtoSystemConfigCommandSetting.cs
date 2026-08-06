namespace GuardianCommunication.Shared.Dto
{
    public class DtoSystemConfigDeviceCommandSetting
    {
        public int SleepBetweenSendsIfCommandNotExistsInMilliSeconds { get; set; }
        public int WaitBetweenCommandSendInMilliseconds { get; set; }
        public int MaxRetryForUserCommand { get; set; }
        public int MaxRetryForOtherCommand { get; set; }
        
    }
}
