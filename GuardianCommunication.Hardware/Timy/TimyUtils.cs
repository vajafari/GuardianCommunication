namespace GuardianCommunication.Hardware.Timy
{
    public class TimyUtils
    {
        public static AttendanceVerificationStyleEnumeration GetVerificationStyle(int verifyMode)
        {
            switch (verifyMode)
            {
                case 1:
                    return AttendanceVerificationStyleEnumeration.Finger;
                case 2:
                    return AttendanceVerificationStyleEnumeration.IdAndPassword;
                case 3:
                    return AttendanceVerificationStyleEnumeration.Card;
                case 4:
                    return AttendanceVerificationStyleEnumeration.CardAndFinger;
                case 5:
                    return AttendanceVerificationStyleEnumeration.CardAndPassword;
                case 6:
                    return AttendanceVerificationStyleEnumeration.CardAndFingerAndPassword;
                case 7:
                    return AttendanceVerificationStyleEnumeration.IdAndFingerAndPassword;
                case 8:
                    return AttendanceVerificationStyleEnumeration.Face;
                default:
                    return AttendanceVerificationStyleEnumeration.Unknown;
            }
        }
    }
}
