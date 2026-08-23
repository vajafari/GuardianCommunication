using GuardianCommunication.Shared.Definition;

namespace GuardianCommunication.Hardware.Zk.ZkConcepts
{
    public static class ZkUtils
    {
        public const string ZkForbiddenPassword = "74219";

        public static AttendanceVerificationStyleEnumeration GetVerificationStyle(int verifyMode)
        {
            switch (verifyMode)
            {
                case 1:
                    return AttendanceVerificationStyleEnumeration.Finger;
                case 2:
                    return AttendanceVerificationStyleEnumeration.JustId;
                case 3:
                    return AttendanceVerificationStyleEnumeration.IdAndPassword;
                case 4:
                    return AttendanceVerificationStyleEnumeration.Card;
                case 5:
                    return AttendanceVerificationStyleEnumeration.FingerAndPassword;
                case 6:
                    return AttendanceVerificationStyleEnumeration.CardAndFinger;
                case 7:
                    return AttendanceVerificationStyleEnumeration.CardAndPassword;
                case 8:
                    return AttendanceVerificationStyleEnumeration.IdAndFinger;
                case 9:
                    return AttendanceVerificationStyleEnumeration.FingerAndPassword;
                case 10:
                    return AttendanceVerificationStyleEnumeration.CardAndFinger;
                case 11:
                    return AttendanceVerificationStyleEnumeration.CardAndPassword;
                case 12:
                    return AttendanceVerificationStyleEnumeration.CardAndFingerAndPassword;
                case 13:
                    return AttendanceVerificationStyleEnumeration.IdAndFingerAndPassword;
                case 14:
                    return AttendanceVerificationStyleEnumeration.FingerAndPassword;
                case 15:
                    return AttendanceVerificationStyleEnumeration.Face;
                case 25:
                    return AttendanceVerificationStyleEnumeration.Palm;
                case 26:
                    return AttendanceVerificationStyleEnumeration.PalmAndCard;
                case 27:
                    return AttendanceVerificationStyleEnumeration.PalmAndFace;
                case 28:
                    return AttendanceVerificationStyleEnumeration.PalmAndFinger;
                default:
                    return AttendanceVerificationStyleEnumeration.Unknown;
            }
        }

    }
}
