namespace GuardianCommunication.Hardware.Suprema.SupremaConcepts.V2
{
	public enum SupremaSdk2VerificationStyleEnumeration
	{
		BiometricOnly = 0,
		BiometricAndPassword = 1,

		CardOnly = 2,
		CardAndBiometric = 3,
		CardAndPassword = 4,
		CardAndBiometricOrPassword = 5,
		CardAndBiometricAndPassword = 6,

		IdAndBiometric = 7,
		IdAndPassword = 8,
		IdAndBiometricOrPassword = 9,
		IdAndBiometricAndPassword = 10,


		BiometricOrPassword = 512,
		BiometricOrCard = 513,
		BiometricOrCardOrPassword = 514,

		DeviceSetting = 515,
		Prohibited = 516,
	}
}
