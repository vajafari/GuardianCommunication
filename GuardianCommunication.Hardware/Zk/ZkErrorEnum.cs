namespace GuardianCommunication.Hardware.Zk
{
    public enum ZkErrorEnum : short
    {
        Successful = 0,
        /// <summary>
        /// خطا در اتصال
        /// </summary>
        ErrorCode1 = 1,
        /// <summary>
        /// ایندکس اثر انگشت وجود دارد
        /// </summary>
        ErrorCode2 = 2,
        /// <summary>
        ///خطای تخصیص بافر
        /// </summary>
        ErrorCode101 = 101,
        /// <summary>
        /// تماس مكرر
        /// </summary>
        ErrorCode102 = 102,
        /// <summary>
        /// مقداردهی اولیه انجام نشد
        /// </summary>
        ErrorCodeNegative1 = -1,
        /// <summary>
        /// خطا در خواندن و نوشتن فایل
        /// </summary>
        ErrorCodeNegative2 = -2,
        /// <summary>
        ///  اندازه اشتباه
        /// </summary>
        ErrorCodeNegative3 = -3,
        /// <summary>
        /// داده از قبل وجود دارد
        /// </summary>
        ErrorCodeNegative5 = -5,
        /// <summary>
        /// گذرواژه نادرست است
        /// </summary>
        ErrorCodeNegative6 = -6,
        /// <summary>
        /// خطای پاسخ
        /// </summary>
        ErrorCodeNegative7 = -7,
        /// <summary>
        /// مهلت دریافت کنید
        /// </summary>
        ErrorCodeNegative8 = -8,
        /// <summary>
        /// طول داده های ارسال شده نادرست است
        /// </summary>
        ErrorCodeNegative10 = -10,
        /// <summary>
        /// عملیات پشتیبانی نمی شود
        /// </summary>
        ErrorCodeNegative100 = -100,
        /// <summary>
        /// خطای نسخه داده
        /// </summary>
        ErrorCodeNegative102 = -102,
        /// <summary>
        /// دستگاه نسخه اشتباه الگوی صورت را برمی گرداند
        /// </summary>
        ErrorCodeNegative103 = -103,
        /// <summary>
        /// دستگاه مشغول است
        /// </summary>
        ErrorCodeNegative201 = -201,
        /// <summary>
        /// وقفه اتصال
        /// </summary>
        ErrorCodeNegative307 = -307,
        /// <summary>
        /// بازگشت برای اجرای دستور ناموفق است
        /// </summary>
        ErrorCodeNegative2001 = -2001,
        /// <summary>
        /// بازگشت داده ها
        /// </summary>
        ErrorCodeNegative2002 = -2002,
        /// <summary>
        /// رویداد ثبت شده رخ داده است
        /// </summary>
        ErrorCodeNegative2003 = -2003,
        /// <summary>
        /// دستور REPEAT را برگردانید
        /// </summary>
        ErrorCodeNegative2004 = -2004,
        /// <summary>
        /// بازگشت فرماندهی UNAUTH
        /// </summary>
        ErrorCodeNegative2005 = -2005,
        /// <summary>
        /// تخصیص حافظه انجام نشد
        /// </summary>
        ErrorCodeNegative4982 = -4982,
        /// <summary>
        /// محاسبه مقدار Hash انجام نشد
        /// </summary>
        ErrorCodeNegative4983 = -4983,
        /// <summary>
        /// نوشتن پرونده ناموفق بود
        /// </summary>
        ErrorCodeNegative4984 = -4984,
        /// <summary>
        /// خواندن پرونده ناموفق بود
        /// </summary>
        ErrorCodeNegative4985 = -4985,
        /// <summary>
        /// پرونده وجود ندارد
        /// </summary>
        ErrorCodeNegative4986 = -4986,
        /// <summary>
        /// سرریز حافظه اختصاص جلسه
        /// </summary>
        ErrorCodeNegative4987 = -4987,
        /// <summary>
        /// فضای حافظه کافی برای جلسه اختصاص داده نشده است
        /// </summary>
        ErrorCodeNegative4988 = -4988,
        /// <summary>
        /// جلسه قادر به تخصیص حافظه نبود
        /// </summary>
        ErrorCodeNegative4989 = -4989,
        /// <summary>
        /// حجم داده پایگاه داده به حد دستگاه رسیده است
        /// </summary>
        ErrorCodeNegative4990 = -4990,
        /// <summary>
        /// هیچ داده مرتبطی در پایگاه داده وجود ندارد
        /// </summary>
        ErrorCodeNegative4991 = -4991,
        /// <summary>
        /// عملیات حذف پایگاه داده انجام نشد
        /// </summary>
        ErrorCodeNegative4992 = -4992,
        /// <summary>
        /// عملیات خواندن پایگاه داده انجام نشد
        /// </summary>
        ErrorCodeNegative4993 = -4993,
        /// <summary>
        ///  عملیات به روز رسانی پایگاه داده انجام نشد
        /// </summary>
        ErrorCodeNegative4994 = -4994,
        /// <summary>
        /// عملیات افزودن پایگاه داده ناموفق بود
        /// </summary>
        ErrorCodeNegative4995 = -4995,
        /// <summary>
        /// پارامتر ارسالی توسط نرم افزار به دستگاه اشتباه است
        /// </summary>
        ErrorCodeNegative4996 = -4996,
        /// <summary>
        ///  طول داده های ارسال شده توسط نرم افزار به دستگاه اشتباه است
        /// </summary>
        ErrorCodeNegative4997 = -4997,
        /// <summary>
        /// خطای پارامتر دستگاه را بنویسید
        /// </summary>
        ErrorCodeNegative4998 = -4998,
        /// <summary>
        /// خطا در خواندن پارامترهای دستگاه
        /// </summary>
        ErrorCodeNegative4999 = -4999,
        /// <summary>
        ///  ایجاد وقفه سوکت (وقفه اتصال)
        /// </summary>
        ErrorCodeNegative12001 = -12001,
        /// <summary>
        /// حافظه کافی نیست
        /// </summary>
        ErrorCodeNegative12002 = -12002,
        /// <summary>
        /// خطای نسخه سوکت
        /// </summary>
        ErrorCodeNegative12003 = -12003,
        /// <summary>
        /// پروتکل غیر TCP
        /// </summary>
        ErrorCodeNegative12004 = -12004,
        /// <summary>
        /// مهلت انتظار
        /// </summary>
        ErrorCodeNegative12005 = -12005,
        /// <summary>
        /// ارسال مهلت زمانی داده
        /// </summary>
        ErrorCodeNegative12006 = -12006,
        /// <summary>
        /// خواندن مهلت زمانی داده
        /// </summary>
        ErrorCodeNegative12007 = -12007,
        /// <summary>
        /// SOCKET قابل خواندن نیست
        /// </summary>
        ErrorCodeNegative12008 = -12008,
        /// <summary>
        /// در انتظار خطای سمافر
        /// </summary>
        ErrorCodeNegative13009 = -13009,
        /// <summary>
        /// تعداد تلاشهای مجدد بیشتر شد
        /// </summary>
        ErrorCodeNegative13010 = -13010,
        /// <summary>
        /// REPLYID
        /// </summary>
        ErrorCodeNegative13011 = -13011,
        /// <summary>
        /// خطای Checksum
        /// </summary>
        ErrorCodeNegative13012 = -13012,
        /// <summary>
        /// منتظر مهلت زمانی semaphore
        /// </summary>
        ErrorCodeNegative13013 = -13013,
        /// <summary>
        /// DIRTY_DATA
        /// </summary>
        ErrorCodeNegative13014 = -13014,
        /// <summary>
        ///  اندازه بافر خیلی کوچک است
        /// </summary>
        ErrorCodeNegative13015 = -13015,
        /// <summary>
        /// طول داده های خوانده شده اشتباه است
        /// </summary>
        ErrorCodeNegative13016 = -13016,
        /// <summary>
        /// داده خواندن نامعتبر است 1
        /// </summary>
        ErrorCodeNegative13017 = -13017,
        /// <summary>
        /// داده خواندن نامعتبر است 2
        /// </summary>
        ErrorCodeNegative13018 = -13018,
        /// <summary>
        /// داده خواندن نامعتبر است 3
        /// </summary>
        ErrorCodeNegative13019 = -13019,
        /// <summary>
        /// داده از دست رفته
        /// </summary>
        ErrorCodeNegative13020 = -13020,
        /// <summary>
        /// خطای مقداردهی اولیه حافظه
        /// </summary>
        ErrorCodeNegative13021 = -13021,
        /// <summary>
        /// مقدار وضعیت صادر شده توسط رابط SetShortkey تکرار می شود
        /// </summary>
        ErrorCodeNegative15001 = -15001,
        /// <summary>
        /// توضیحات تکراری فراخوانی رابط SetShortkey
        /// </summary>
        ErrorCodeNegative15002 = -15002,
        /// <summary>
        /// منوی ثانویه در دستگاه باز نمی شود و نیازی به صدور آن نیست
        /// </summary>
        ErrorCodeNegative15003 = -15003,
        /// <summary>
        /// خطا در دریافت ساختار جدول
        /// </summary>
        ErrorCodeNegative15100 = -15100,
        /// <summary>
        /// قسمت شرط در ساختار جدول وجود ندارد
        /// </summary>
        ErrorCodeNegative15101 = -15101,
        /// <summary>
        /// تعداد کل قسمت ها متناقض است
        /// </summary>
        ErrorCodeNegative15102 = -15102,
        /// <summary>
        /// مرتب سازی درست متناقض است
        /// </summary>
        ErrorCodeNegative15103 = -15103,
        /// <summary>
        /// خطای تخصیص حافظه
        /// </summary>
        ErrorCodeNegative15104 = -15104,
        /// <summary>
        /// خطای داده هنگام تجزیه داده ها
        /// </summary>
        ErrorCodeNegative15105 = -15105,
        /// <summary>
        /// داده های صادر شده بیش از 4M است ، سرریز داده ها
        /// </summary>
        ErrorCodeNegative15106 = -15106,
        /// <summary>
        /// گزینه OPTIONS نامعتبر است
        /// </summary>
        ErrorCodeNegative15108 = -15108,
        /// <summary>
        /// خطای داده هنگام تجزیه داده ها ، شناسه جدول یافت نمی شود
        /// </summary>
        ErrorCodeNegative5113 = -15113,
        /// <summary>
        /// تعداد قسمتها کمتر از یا برابر با 0 است و داده های برگشتی غیر عادی هستند
        /// </summary>
        ErrorCodeNegative5114 = -15114,
        /// <summary>
        /// عداد کل قسمتهای جدول با تعداد کل قسمتهای تعیین شده توسط خود داده مغایرت دارد و داده ها غیر عادی است
        /// </summary>
        ErrorCodeNegative5115 = -15115,
        


    }
}
