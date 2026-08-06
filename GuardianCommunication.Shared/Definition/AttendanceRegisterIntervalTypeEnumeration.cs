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
		/// کنترل تردد
		/// </summary>
		AccessControl = 2,

		/// <summary>
		/// سیستم حضور و غیاب
		/// </summary>
		TimeAndAttendance = 4,

		/// <summary>
		/// سیستم پارکینگ
		/// </summary>
		Parking = 8,
	}

}
