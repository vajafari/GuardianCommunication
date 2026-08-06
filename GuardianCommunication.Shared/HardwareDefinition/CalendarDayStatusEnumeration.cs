namespace GuardianCommunication.Shared.HardwareDefinition
{
	/// <summary>
	/// وضعیت روز در تقویم رسمی سازمان
	/// </summary>
	public enum CalendarDayStatusEnumeration : short
	{
		/// <summary>
		/// عادی
		/// </summary>
		Normal = 0,

		/// <summary>
		/// تعطیل رسمی
		/// </summary>
		Official = 500,

		/// <summary>
		/// تعطیلات آخر هفته مثل جمعه ها
		/// </summary>
		Weekends = 501,

		/// <summary>
		/// تعطیلاتی که در اثر مناسبت های خاص و به صورت پیش بینی نشده تعطیل می گردند
		/// </summary>
		Special = 502
	}
}
