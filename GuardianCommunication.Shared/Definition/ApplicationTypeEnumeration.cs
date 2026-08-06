using System;

namespace GuardianCommunication.Shared.Definition
{
	/// <summary>
	/// برای ردیابی تغییرات در پایگاه داده می بایست منبع درخواست هایی که به 
	/// سرور می رسد مشخص باشد. این فهرست منبع درخواست ها را مشخص می کند
	/// </summary>
	[Flags]
	public enum ApplicationTypeEnumeration
	{

        /// <summary>
        /// سیستم پایه
        /// </summary>
        Base = 1 << 0,

        /// <summary>
        /// کنترل تردد
        /// </summary>
        AccessControl = 1 << 1,

        /// <summary>
        /// سیستم حضور و غیاب
        /// </summary>
        TimeAndAttendance = 1 << 2,

        /// <summary>
        /// سیستم پارکینگ
        /// </summary>
        Parking = 1 << 3,

        /// <summary>
        /// مراجعین
        /// </summary>
        Visitor = 1 << 4,

        /// <summary>
        /// سیستم رستوران
        /// </summary>
        Self = 1 << 5,

        /// <summary>
        /// حراست
        /// </summary>
        Protection = 1 << 6,

        /// <summary>
        /// ورود و خروج کالا
        /// </summary>
        WareTransition = 1 << 7,

        /// <summary>
        /// گردش کار
        /// </summary>
        WorkFlow = 1 << 8,

        /// <summary>
        /// اسانسور
        /// </summary>
        Elevator = 1 << 9,

        /// <summary>
        /// کمدداری
        /// </summary>
        Cabinet = 1 << 10,

        /// <summary>
        /// گزارش ساز
        /// </summary>
        ReportMaker = 1 << 11,

        /// <summary>
        /// گزارش ساز
        /// </summary>
        FaceDetection = 1 << 12,

        /// <summary>
        /// تمامی نرم افزارها
        /// </summary>
        All = int.MaxValue
	}

}
