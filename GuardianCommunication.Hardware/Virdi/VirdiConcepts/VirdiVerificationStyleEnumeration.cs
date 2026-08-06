using System;

namespace GuardianCommunication.Hardware.Virdi.VirdiConcepts
{
	[Flags]
	public enum VirdiVerificationStyleEnumeration
	{
		None = 0,
		IsAndOperation = 1,
		IsFinger = 2,
		IsCard = 4,
		IsPassword = 8,
		IsFace = 16,
        IsIris = 32
	}
}
