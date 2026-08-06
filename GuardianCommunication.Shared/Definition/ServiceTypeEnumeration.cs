using System;

namespace GuardianCommunication.Shared.Definition
{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum ServiceTypeEnumeration
	{
		
		/// <summary>
		/// تجمیع شده با کارنما
		/// </summary>
		KarnamaDependant = 1,

		/// <summary>
		/// مستقل
		/// </summary>
		Independant = 2,

	}

}
