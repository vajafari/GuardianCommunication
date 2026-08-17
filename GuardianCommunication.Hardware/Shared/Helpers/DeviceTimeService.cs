// ============================================================================
//  DeviceTimeService
// ----------------------------------------------------------------------------
//  سرویس تبدیل زمان بین سرور (UTC) و سخت‌افزارهای کنترل تردد (تایم لوکالِ دستگاه).
//
//  وابستگی:  NodaTime  ->  dotnet add package NodaTime
//  سازگاری:  .NET Framework 4.8  (NodaTime روی netstandard2.0 کار می‌کند)
//
//  دو مفهومِ کاملاً متفاوت اینجا مدیریت می‌شود:
//
//   1) Instant  (لحظه‌ای در زمان)  -> زمانِ تردد. با UTC ذخیره می‌شود.
//   2) Calendar Date (تاریخِ تقویمی) -> تاریخ شروع/پایان اعتبار. خام ذخیره می‌شود
//      و فقط هنگام ارسال به دستگاه به یک لحظه باز می‌شود.
// ============================================================================

using System;
using NodaTime;

namespace GuardianCommunication.Hardware.Shared.Helpers
{
    /// <summary>
    /// حالتِ برخورد با ساعت‌های نامعتبر/مبهمِ DST هنگام تبدیلِ زمانِ تردد به UTC.
    /// </summary>
    public enum DstResolution
    {
        /// <summary>هیچ‌وقت استثنا نمی‌دهد؛ ساعت نامعتبر را جلو می‌برد و مبهم را به وقوعِ اول نگاشت می‌کند.</summary>
        Lenient,

        /// <summary>روی ساعتِ نامعتبر یا مبهم استثنا پرتاب می‌کند (برای لاگ‌گیری و کنترل صریح).</summary>
        Strict
    }

    /// <summary>
    /// تفسیرِ «تاریخ پایان اعتبار» هنگام باز کردن به یک لحظه برای ارسال به دستگاه.
    /// </summary>
    public enum EndOfDayMode
    {
        /// <summary>آخرین ثانیه‌ی همان روز، مثلاً 23:59:59 (بازه‌ی بسته/inclusive).</summary>
        LastSecondOfDay,

        /// <summary>ابتدای روزِ بعد، مثلاً 00:00:00 روز بعد (بازه‌ی نیم‌باز/exclusive).</summary>
        StartOfNextDay
    }

    /// <summary>
    /// سرویس مرزی بین سرور و سخت‌افزار برای تبدیل زمان.
    /// نمونه‌ی این کلاس thread-safe و بدون state است؛ می‌توان یک نمونه را
    /// به‌صورت singleton نگه داشت.
    /// </summary>
    public sealed class DeviceTimeService
    {
        private readonly IDateTimeZoneProvider _tzProvider;

        public DeviceTimeService()
            : this(DateTimeZoneProviders.Tzdb)
        {
        }

        /// <summary>
        /// امکان تزریقِ provider سفارشی (مثلاً برای تست یا نسخه‌ی خاصِ tzdb).
        /// </summary>
        public DeviceTimeService(IDateTimeZoneProvider tzProvider)
        {
            _tzProvider = tzProvider ?? throw new ArgumentNullException(nameof(tzProvider));
        }

        // --------------------------------------------------------------------
        //  کمکی: گرفتنِ تایم‌زون با خطای واضح در صورت نامعتبر بودنِ شناسه.
        // --------------------------------------------------------------------
        private DateTimeZone GetZone(string ianaTimeZoneId)
        {
            if (string.IsNullOrWhiteSpace(ianaTimeZoneId))
                throw new ArgumentException("Time zone id is required.", nameof(ianaTimeZoneId));

            var zone = _tzProvider.GetZoneOrNull(ianaTimeZoneId);
            if (zone == null)
                throw new ArgumentException(
                    $"Unknown IANA time zone id: '{ianaTimeZoneId}'.", nameof(ianaTimeZoneId));

            return zone;
        }

        // ====================================================================
        //  بخش ۱ — زمانِ تردد (لحظه‌ای در زمان)
        // ====================================================================

        /// <summary>
        /// ساعتی که از سخت‌افزار می‌گیری (تایم لوکالِ دستگاه) -> UTC برای ذخیره در دیتابیس.
        /// </summary>
        /// <param name="deviceLocalTime">زمانِ دیوارِ دستگاه. مقدارِ Kind نادیده گرفته می‌شود.</param>
        /// <param name="ianaTimeZoneId">شناسه‌ی IANA تایم‌زونِ دستگاه، مثل "Asia/Dubai".</param>
        /// <param name="resolution">نحوه‌ی برخورد با ساعت‌های DST نامعتبر/مبهم.</param>
        public DateTime DeviceTimeToUtc(
            DateTime deviceLocalTime,
            string ianaTimeZoneId,
            DstResolution resolution = DstResolution.Lenient)
        {
            var zone = GetZone(ianaTimeZoneId);

            var local = LocalDateTime.FromDateTime(deviceLocalTime);

            var zoned = resolution == DstResolution.Strict
                ? local.InZoneStrictly(zone)   // پرتابِ SkippedTimeException / AmbiguousTimeException
                : local.InZoneLeniently(zone);

            return zoned.ToDateTimeUtc();       // DateTime با Kind = Utc
        }

        /// <summary>
        /// ساعت UTC از دیتابیس -> تایم لوکالِ دستگاه برای ارسال به سخت‌افزار.
        /// این جهت هیچ‌وقت مبهم نیست و همیشه یک جوابِ یکتا دارد.
        /// </summary>
        /// <param name="utcTime">زمان UTC. باید Kind=Utc یا Unspecified باشد.</param>
        /// <param name="ianaTimeZoneId">شناسه‌ی IANA تایم‌زونِ دستگاه.</param>
        public DateTime UtcToDeviceTime(DateTime utcTime, string ianaTimeZoneId)
        {
            var zone = GetZone(ianaTimeZoneId);

            if (utcTime.Kind == DateTimeKind.Local)
                throw new ArgumentException(
                    "Expected a UTC or Unspecified DateTime, but got Local.", nameof(utcTime));

            var asUtc = DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
            var instant = Instant.FromDateTimeUtc(asUtc);

            var zoned = instant.InZone(zone);

            return zoned.ToDateTimeUnspecified(); // DateTime با Kind = Unspecified
        }

        /// <summary>
        /// یک تاریخِ تقویمی -> ابتدای روز (00:00) در تایم‌زونِ داده‌شده.
        /// عام است: تاریخ شروعِ اعتبار، مبدأِ یک روزِ تقویم، شروعِ بازه‌ی گزارش و ...
        /// AtStartOfDay حالتِ نادرِ «نیمه‌شبی که به‌خاطر DST وجود نداشته» را هم درست مدیریت می‌کند.
        /// </summary>
        public DateTime DateToStartOfDayInZone(LocalDate date, string ianaTimeZoneId)
        {
            var zone = GetZone(ianaTimeZoneId);
            var startOfDay = zone.AtStartOfDay(date);
            return startOfDay.ToDateTimeUnspecified();
        }

        /// <summary>
        /// یک تاریخِ تقویمی -> پایانِ روز در تایم‌زونِ داده‌شده.
        /// عام است: تاریخ پایانِ اعتبار، انتهای یک روزِ تقویم، پایانِ بازه‌ی گزارش و ...
        /// حالتِ inclusive یا exclusive را با <paramref name="mode"/> انتخاب کن،
        /// بسته به اینکه مصرف‌کننده (دستگاه/کوئری) بازه را چطور تفسیر می‌کند.
        /// </summary>
        public DateTime DateToEndOfDayInZone(
            LocalDate date,
            string ianaTimeZoneId,
            EndOfDayMode mode = EndOfDayMode.LastSecondOfDay)
        {
            var zone = GetZone(ianaTimeZoneId);

            if (mode == EndOfDayMode.StartOfNextDay)
            {
                var nextStart = zone.AtStartOfDay(date.PlusDays(1));
                return nextStart.ToDateTimeUnspecified();     // 00:00:00 روزِ بعد
            }

            var endLocal = date + new LocalTime(23, 59, 59);
            var zoned = endLocal.InZoneLeniently(zone);
            return zoned.ToDateTimeUnspecified();             // 23:59:59 همان روز
        }

        // ====================================================================
        //  بخش ۳ — آفستِ تایم‌زون نسبت به UTC
        //  دقت: آفست برای مناطقِ DST‌دار ثابت نیست و به لحظه بستگی دارد.
        //  پس همیشه می‌پرسیم «آفست در چه لحظه‌ای؟». برای تهرانِ امروز (بدونِ DST)
        //  همیشه +03:30 است، ولی این متدها برای همه‌ی مناطق درست کار می‌کنند.
        // ====================================================================

        /// <summary>
        /// آفستِ تایم‌زون نسبت به UTC در یک لحظه‌ی مشخص.
        /// </summary>
        /// <param name="ianaTimeZoneId">شناسه‌ی IANA، مثل "Asia/Tehran".</param>
        /// <param name="atUtc">
        /// لحظه‌ای که آفست در آن حساب می‌شود. اگر ندهی، «هم‌اکنون» در نظر گرفته می‌شود.
        /// برای مناطقِ DST‌دار خروجی بسته به این لحظه فرق می‌کند.
        /// </param>
        public Offset GetUtcOffset(string ianaTimeZoneId, DateTime? atUtc = null)
        {
            var zone = GetZone(ianaTimeZoneId);

            Instant instant;
            if (atUtc.HasValue)
            {
                var asUtc = DateTime.SpecifyKind(atUtc.Value, DateTimeKind.Utc);
                instant = Instant.FromDateTimeUtc(asUtc);
            }
            else
            {
                instant = SystemClock.Instance.GetCurrentInstant();
            }

            return zone.GetUtcOffset(instant);
        }

        /// <summary>
        /// آفستِ تایم‌زون به‌صورت رشته‌ی خوانا مثل "+03:30" یا "-05:00" یا "+00:00".
        /// </summary>
        /// <param name="ianaTimeZoneId">شناسه‌ی IANA، مثل "Asia/Tehran".</param>
        /// <param name="atUtc">لحظه‌ی محاسبه؛ اگر ندهی، «هم‌اکنون».</param>
        public string GetUtcOffsetString(string ianaTimeZoneId, DateTime? atUtc = null)
        {
            var offset = GetUtcOffset(ianaTimeZoneId, atUtc);

            var totalMinutes = offset.Seconds / 60;   // می‌تواند منفی باشد
            var sign = totalMinutes < 0 ? '-' : '+';
            var abs = Math.Abs(totalMinutes);

            return $"{sign}{abs / 60:D2}:{abs % 60:D2}";
        }



        /// <summary>DateTime (فقط بخشِ تاریخِ آن) -> LocalDate. بخشِ ساعت دور ریخته می‌شود.</summary>
        public static LocalDate DateTimeToLocalDate(DateTime value)
            => new LocalDate(value.Year, value.Month, value.Day);

        /// <summary>LocalDate -> DateTime با ساعتِ 00:00:00 و Kind=Unspecified (مناسبِ ستونِ date).</summary>
        public static DateTime LocalDateToDateTime(LocalDate date)
            => new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Unspecified);
    }
}
