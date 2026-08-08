using System;

namespace GuardianCommunication.Shared.Definition
{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum AttendanceRegisterIntervalTypeEnumeration
	{
		/// <summary>
		/// در کلیه سیستم ها
		/// </summary>
		General = 1,

		/// <summary>
		/// سیستم پارکینگ
		/// </summary>
		Parking = 2,
	}

}
