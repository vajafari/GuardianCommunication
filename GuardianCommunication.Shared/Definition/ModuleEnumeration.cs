using System;

namespace GuardianCommunication.Shared.Definition
{

    /// <summary>
    /// برای ردیابی تغییرات در پایگاه داده می بایست منبع درخواست هایی که به 
    /// سرور می رسد مشخص باشد. این فهرست منبع درخواست ها را مشخص می کند
    /// </summary>
    [Flags]
    public enum ModuleEnumeration
    {
        /// <summary>
        /// سیستم پایه
        /// </summary>
        Base = 1 << 0,

        /// <summary>
        /// کنترل تردد
        /// </summary>
        Accounting = 1 << 1,

        /// <summary>
        /// سیستم حضور و غیاب
        /// </summary>
        Payment = 1 << 2,

        /// <summary>
        /// مراجعین
        /// </summary>
        Visitor = 1 << 3,

        /// <summary>
        /// آسانسور
        /// </summary>
        Elevator = 1 << 4,

        /// <summary>
        /// کمدداری
        /// </summary>
        Cabinet = 1 << 5,

        /// <summary>
        /// گزارش ساز
        /// </summary>
        ReportMaker = 1 << 6,

        /// <summary>
        /// گزارش ساز
        /// </summary>
        Parking = 1 << 8,

        /// <summary>
        /// تمامی ماژول ها
        /// </summary>

        All = int.MaxValue
    }
}