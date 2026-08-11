namespace GuardianCommunication.Hardware.Zk.ZkConcepts
{
    internal static class ZkCommunicationHelpers
    {

        internal static ZkPushVerificationStyleEnumeration MapSdkVerificationStyleToPush(
            ZkVerificationStyleEnumeration verificationStyle)
        {
            switch (verificationStyle)
            {
                case ZkVerificationStyleEnumeration.GroupVerify:
                    return ZkPushVerificationStyleEnumeration.GroupVerify;
                //case ZkVerificationStyleEnumeration.FingerPrintOrPasswordOrRfCard:
                //	return ZkPushVerificationStyleEnumeration.FingerPrintOrPasswordOrRfCard;
                case ZkVerificationStyleEnumeration.FingerPrint:
                    return ZkPushVerificationStyleEnumeration.FingerPrint;
                case ZkVerificationStyleEnumeration.Pin:
                    return ZkPushVerificationStyleEnumeration.Pin;
                case ZkVerificationStyleEnumeration.Password:
                    return ZkPushVerificationStyleEnumeration.Password;
                case ZkVerificationStyleEnumeration.RfCard:
                    return ZkPushVerificationStyleEnumeration.RfCard;
                case ZkVerificationStyleEnumeration.FingerPrintOrPassword:
                    return ZkPushVerificationStyleEnumeration.FingerPrintOrPassword;
                case ZkVerificationStyleEnumeration.FingerPrintOrRfCard:
                    return ZkPushVerificationStyleEnumeration.FingerPrintOrRfCard;
                case ZkVerificationStyleEnumeration.PasswordOrRfCard:
                    return ZkPushVerificationStyleEnumeration.PasswordOrRfCard;
                case ZkVerificationStyleEnumeration.PinAndFingerPrint:
                    return ZkPushVerificationStyleEnumeration.PinAndFingerPrint;


                //case ZkVerificationStyleEnumeration.fin:
                //    return ZkPushVerificationStyleEnumeration.PinAndFingerPrint;


                //case ZkVerificationStyleEnumeration.FingerPrintAndPasswordOrRfCard:
                //	return ZkPushVerificationStyleEnumeration.FingerPrintAndPasswordOrRfCard;
                case ZkVerificationStyleEnumeration.FingerPrintAndRfCard:
                    return ZkPushVerificationStyleEnumeration.FingerPrintAndRfCard;
                case ZkVerificationStyleEnumeration.PasswordAndRfCard:
                    return ZkPushVerificationStyleEnumeration.PasswordAndRfCard;
                case ZkVerificationStyleEnumeration.FingerPrintAndPasswordAndRfCard:
                    return ZkPushVerificationStyleEnumeration.FingerPrintAndPasswordAndRfCard;
                case ZkVerificationStyleEnumeration.PinAndFingerPrintAndPassword:
                    return ZkPushVerificationStyleEnumeration.PinAndFingerPrintAndPassword;
                //case ZkVerificationStyleEnumeration.FingerPrintAndRfCardOrPin:
                //	return ZkPushVerificationStyleEnumeration.FingerPrintAndRfCardOrPin;
                case ZkVerificationStyleEnumeration.Face:
                    return ZkPushVerificationStyleEnumeration.Face;
                case ZkVerificationStyleEnumeration.FaceAndFingerPrint:
                    return ZkPushVerificationStyleEnumeration.FaceAndFingerPrint;
                case ZkVerificationStyleEnumeration.FaceAndPassword:
                    return ZkPushVerificationStyleEnumeration.FaceAndPassword;
                case ZkVerificationStyleEnumeration.FaceAndRfCard:
                    return ZkPushVerificationStyleEnumeration.FaceAndRfCard;
                case ZkVerificationStyleEnumeration.FaceAndFingerPrintAndRfCard:
                    return ZkPushVerificationStyleEnumeration.FaceAndFingerPrintAndRfCard;
                case ZkVerificationStyleEnumeration.FaceAndFingerPrintAndPassword:
                    return ZkPushVerificationStyleEnumeration.FaceAndFingerPrintAndPassword;
                case ZkVerificationStyleEnumeration.Palm:
                    return ZkPushVerificationStyleEnumeration.Palm;
                case ZkVerificationStyleEnumeration.PalmAndCard:
                    return ZkPushVerificationStyleEnumeration.PalmAndCard;
                case ZkVerificationStyleEnumeration.PalmAndFace:
                    return ZkPushVerificationStyleEnumeration.PalmAndFace;
                case ZkVerificationStyleEnumeration.PalmAndFinger:
                    return ZkPushVerificationStyleEnumeration.PalmAndFinger;
                case ZkVerificationStyleEnumeration.PalmAndFingerAndFace:
                    return ZkPushVerificationStyleEnumeration.PalmAndFingerAndFace;
                default:
                    return ZkPushVerificationStyleEnumeration.GroupVerify;
            }
        }

        internal static ZkVerificationStyleEnumeration MapPushVerificationStyleToSdk(
            ZkPushVerificationStyleEnumeration verificationStyle)
        {
            switch (verificationStyle)
            {
                case ZkPushVerificationStyleEnumeration.GroupVerify:
                    return ZkVerificationStyleEnumeration.GroupVerify;
                //case ZkVerificationStyleEnumeration.FingerPrintOrPasswordOrRfCard:
                //	return ZkPushVerificationStyleEnumeration.FingerPrintOrPasswordOrRfCard;
                case ZkPushVerificationStyleEnumeration.FingerPrint:
                    return ZkVerificationStyleEnumeration.FingerPrint;
                case ZkPushVerificationStyleEnumeration.Pin:
                    return ZkVerificationStyleEnumeration.Pin;
                case ZkPushVerificationStyleEnumeration.Password:
                    return ZkVerificationStyleEnumeration.Password;
                case ZkPushVerificationStyleEnumeration.RfCard:
                    return ZkVerificationStyleEnumeration.RfCard;
                case ZkPushVerificationStyleEnumeration.FingerPrintOrPassword:
                    return ZkVerificationStyleEnumeration.FingerPrintOrPassword;
                case ZkPushVerificationStyleEnumeration.FingerPrintOrRfCard:
                    return ZkVerificationStyleEnumeration.FingerPrintOrRfCard;
                case ZkPushVerificationStyleEnumeration.PasswordOrRfCard:
                    return ZkVerificationStyleEnumeration.PasswordOrRfCard;
                case ZkPushVerificationStyleEnumeration.PinAndFingerPrint:
                    return ZkVerificationStyleEnumeration.PinAndFingerPrint;
                //case ZkVerificationStyleEnumeration.FingerPrintAndPasswordOrRfCard:
                //	return ZkPushVerificationStyleEnumeration.FingerPrintAndPasswordOrRfCard;
                case ZkPushVerificationStyleEnumeration.FingerPrintAndRfCard:
                    return ZkVerificationStyleEnumeration.FingerPrintAndRfCard;
                case ZkPushVerificationStyleEnumeration.PasswordAndRfCard:
                    return ZkVerificationStyleEnumeration.PasswordAndRfCard;
                case ZkPushVerificationStyleEnumeration.FingerPrintAndPasswordAndRfCard:
                    return ZkVerificationStyleEnumeration.FingerPrintAndPasswordAndRfCard;
                case ZkPushVerificationStyleEnumeration.PinAndFingerPrintAndPassword:
                    return ZkVerificationStyleEnumeration.PinAndFingerPrintAndPassword;
                //case ZkVerificationStyleEnumeration.FingerPrintAndRfCardOrPin:
                //	return ZkPushVerificationStyleEnumeration.FingerPrintAndRfCardOrPin;
                case ZkPushVerificationStyleEnumeration.Face:
                    return ZkVerificationStyleEnumeration.Face;
                case ZkPushVerificationStyleEnumeration.FaceAndFingerPrint:
                    return ZkVerificationStyleEnumeration.FaceAndFingerPrint;
                case ZkPushVerificationStyleEnumeration.FaceAndPassword:
                    return ZkVerificationStyleEnumeration.FaceAndPassword;
                case ZkPushVerificationStyleEnumeration.FaceAndRfCard:
                    return ZkVerificationStyleEnumeration.FaceAndRfCard;
                case ZkPushVerificationStyleEnumeration.FaceAndFingerPrintAndRfCard:
                    return ZkVerificationStyleEnumeration.FaceAndFingerPrintAndRfCard;
                case ZkPushVerificationStyleEnumeration.FaceAndFingerPrintAndPassword:
                    return ZkVerificationStyleEnumeration.FaceAndFingerPrintAndPassword;
                case ZkPushVerificationStyleEnumeration.Palm:
                    return ZkVerificationStyleEnumeration.Palm;
                case ZkPushVerificationStyleEnumeration.PalmAndCard:
                    return ZkVerificationStyleEnumeration.PalmAndCard;
                case ZkPushVerificationStyleEnumeration.PalmAndFace:
                    return ZkVerificationStyleEnumeration.PalmAndFace;
                case ZkPushVerificationStyleEnumeration.PalmAndFinger:
                    return ZkVerificationStyleEnumeration.PalmAndFinger;
                case ZkPushVerificationStyleEnumeration.PalmAndFingerAndFace:
                    return ZkVerificationStyleEnumeration.PalmAndFingerAndFace;
                default:
                    return ZkVerificationStyleEnumeration.GroupVerify;
            }
        }

    }
}
