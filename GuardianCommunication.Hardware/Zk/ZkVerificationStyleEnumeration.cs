namespace GuardianCommunication.Hardware.Zk
{
	public enum ZkVerificationStyleEnumeration
	{
		GroupVerify = 0,
		FingerPrintOrPasswordOrRfCard = 128,
		FingerPrint = 129,
		Pin = 130,
		Password = 131,
		RfCard = 132,
		FingerPrintOrPassword = 133,
		FingerPrintOrRfCard = 134,
		PasswordOrRfCard = 135,
		PinAndFingerPrint = 136,
		FingerPrintAndPasswordOrRfCard = 137,
		FingerPrintAndRfCard = 138,
		PasswordAndRfCard = 139,
		FingerPrintAndPasswordAndRfCard = 140,
		PinAndFingerPrintAndPassword = 141,
		FingerPrintAndRfCardOrPin = 142,
		Face = 143,
		FaceAndFingerPrint = 144,
		FaceAndPassword = 145,
		FaceAndRfCard = 146,
		FaceAndFingerPrintAndRfCard = 147,
		FaceAndFingerPrintAndPassword = 148,
		Palm = 300,
		PalmAndCard = 301,
		PalmAndFace = 302,
		PalmAndFinger = 303,
		PalmAndFingerAndFace = 304,
	}
}
