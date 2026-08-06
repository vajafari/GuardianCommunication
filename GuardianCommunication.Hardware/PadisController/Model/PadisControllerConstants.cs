namespace GuardianCommunication.Hardware.PadisController.Model
{
    public static class PadisControllerConstants
    {
#if DEBUG
        public const int AttendanceLimit = 10;
        public const int UserLimit = 2;
#else
        public const int AttendanceLimit = 400;
        public const int UserLimit = 200;

#endif
    }
}
