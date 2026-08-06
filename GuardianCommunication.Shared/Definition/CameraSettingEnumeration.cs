using System;
using System.Runtime.Serialization;

namespace GuardianCommunication.Shared.Definition
{
    [Flags]
    [DataContract]
    public enum CameraSettingEnumeration : long
    {
        None = 0,

        /// <summary>
        /// جمع‌آوری تردد های دوربین فعال است
        /// </summary>
        AutoCollectActive = 1,

        /// <summary>
        /// می بایست لیست خودرو های مجاز و نامجاز به دوربین ارسال شود
        /// نیازمند ارسال لیست خودروهای مجاز نامجاز
        /// </summary>
        SendValidList = 2,

        /// <summary>
        /// می بایست به صورت آنلاین به دوربین متصل شویم و تردد ها را به صورت لایو ارسال نماییم
        /// نیازمند اتصال و ارسال آنلاین ترددها
        /// </summary>
        SendAttendanceLive = 4,

        /// <summary>
        /// آیا دوربین سیاه و سفید است
        /// </summary>
        IsBlackAndWhite = 8,

        /// <summary>
        /// دریافت عکس در هنگام جمع‌آوری خودکاری ترددها
        /// </summary>
        GetImageAtAutoAttendanceCollect = 16

    }
}
