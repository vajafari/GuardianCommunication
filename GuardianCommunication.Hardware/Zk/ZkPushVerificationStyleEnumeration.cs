namespace GuardianCommunication.Hardware.Zk
{
	public enum ZkPushVerificationStyleEnumeration
	{
		GroupVerify = 0,
		FingerPrint = 1,
		Pin = 2,
		Password = 3,
		RfCard = 4,
		FingerPrintOrPassword = 5,
		FingerPrintOrRfCard = 6,
		PasswordOrRfCard = 7,
		PinAndFingerPrint = 8,
		FingerPrintAndRfCard = 10,
		PasswordAndRfCard = 11,
		FingerPrintAndPasswordAndRfCard= 12,
		PinAndFingerPrintAndPassword = 13,
		Face = 15,
		FaceAndFingerPrint = 16,
		FaceAndPassword = 17,
		FaceAndRfCard = 18,
		FaceAndFingerPrintAndRfCard = 19,
		FaceAndFingerPrintAndPassword = 20,
		Palm = 25,
		PalmAndCard = 26,
		PalmAndFace = 27,
		PalmAndFinger = 28,
		PalmAndFingerAndFace = 29,
		Other = 200,
	}
}
