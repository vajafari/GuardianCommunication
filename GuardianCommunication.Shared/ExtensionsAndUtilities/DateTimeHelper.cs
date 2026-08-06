using System;
using System.Globalization;
using GuardianCommunication.Shared.Definition;
using GuardianCommunication.Shared.HardwareDefinition;
using MD.PersianDateTime;

namespace GuardianCommunication.Shared.ExtensionsAndUtilities
{
    public static class DateTimeHelper
    {

        #region Time
        static readonly GregorianCalendar GCalendar = new GregorianCalendar();

        public static int GetWeekOfMonth(this DateTime time)
        {
            var first = new DateTime(time.Year, time.Month, 1);
            return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        private static int GetWeekOfYear(this DateTime time)
        {
            return GCalendar.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        public static string FormatIntAsTimeString(this int time)
        {
            var hourMinute = ExtractHourAndMinute(time);
            return hourMinute[0].ToString("00") + hourMinute[1].ToString("00");
        }

        public static string FormatIntAsTimeString(this int time, string separator)
        {
            var hourMinute = ExtractHourAndMinute(time);
            return $"{hourMinute[0]:00}{(separator.IsNotNullOrEmpty() ? separator : string.Empty)}{hourMinute[1]:00}";
        }

        public static bool IsTimeValid(this int time)
        {
            if (time % 100 > ServiceConstants.GeneralMaxValueForMinute)
                return false;
            return time / 100 < 24;
        }

        public static bool IsTimeDurationValid(this int timeDuration)
        {
            var minutes = Math.Abs(timeDuration % 100);
            return minutes <= ServiceConstants.GeneralMaxValueForMinute;
        }

        public static bool IsTimeDurationValid(this int? timeDuration)
        {
            return !timeDuration.HasValue || IsTimeDurationValid(timeDuration.Value);
        }

        public static bool IsInRangeHourMinute(this int timeDurationToEvaluate, int startTimeDuration, int endTimeDuration)
        {
            if (!IsTimeDurationValid(timeDurationToEvaluate))
                throw new ArgumentException($"Time duration is not valid. timeDurationToEvaluate: {timeDurationToEvaluate}");
            if (!IsTimeDurationValid(startTimeDuration))
                throw new ArgumentException($"Time duration is not valid. startTimeDuration: {startTimeDuration}");
            if (!IsTimeDurationValid(endTimeDuration))
                throw new ArgumentException($"Time duration is not valid. endTimeDuration: {endTimeDuration}");

            return ConvertTimeDurationToMinutes(timeDurationToEvaluate).IsInRange(ConvertTimeDurationToMinutes(startTimeDuration), ConvertTimeDurationToMinutes(endTimeDuration));

        }

        public static bool IsInRangeMinuteSecond(this int value, int minValue, int maxValue)
        {
            var result = true;

            var minuteForValueAbs = Math.Abs(value % 100);
            var minuteForMinValueAbs = Math.Abs(minValue % 100);
            var minuteForMaxValueAbs = Math.Abs(maxValue % 100);

            if (minuteForValueAbs > ServiceConstants.GeneralMaxValueForSecond || minuteForValueAbs < ServiceConstants.GeneralMinValueForSecond)
                throw new ArgumentException($"Munite second is not valid. minuteSecondToEvaluate: {value}");

            if (minuteForMinValueAbs > ServiceConstants.GeneralMaxValueForSecond || minuteForMinValueAbs < ServiceConstants.GeneralMinValueForSecond)
                throw new ArgumentException($"Munite second is not valid. minuteSecondToEvaluate: {maxValue}");

            if (minuteForMaxValueAbs > ServiceConstants.GeneralMaxValueForSecond || minuteForMaxValueAbs < ServiceConstants.GeneralMinValueForSecond)
                throw new ArgumentException($"Munite second is not valid. minuteSecondToEvaluate: {minValue}");


            if (ConvertTimeDurationToMinutes(value) < ConvertTimeDurationToMinutes(minValue)
                || ConvertTimeDurationToMinutes(value) > ConvertTimeDurationToMinutes(maxValue))
            {
                result = false;
            }

            return result;
        }

        public static int? ConvertTimeDurationToMinutes(this int? timeDuration)
        {
            if (!timeDuration.HasValue) return null;
            return ConvertTimeDurationToMinutes(timeDuration.Value);
        }

        public static int ConvertTimeDurationToMinutes(this int timeDuration)
        {
            if (!IsTimeDurationValid(timeDuration))
                throw new ArgumentException($"Time duration is not valid. time value {timeDuration}");
            return ((timeDuration / 100) * 60) + (timeDuration % 100);
        }

        public static int? ConvertMinutesToTimeDuration(this int? minutes)
        {
            if (!minutes.HasValue) return null;
            return ConvertMinutesToTimeDuration(minutes.Value);
        }

        public static int ConvertMinutesToTimeDuration(this int minutes)
        {
            return ((minutes / 60) * 100) + (minutes % 60);
        }

        /// <summary>
        /// Index 0 = Hour
        /// Index 1 = Minute
        /// </summary>
        public static int[] ExtractHourAndMinute(this int timeDuration)
        {
            return new[] { timeDuration / 100, timeDuration % 100 };
        }

        /// <summary>
        /// Index 0 = Minute
        /// Index 1 = Second
        /// </summary>
        public static int[] ExtractMinuteAndSecond(this int timeDuration)
        {
            return ExtractHourAndMinute(timeDuration);
        }

        public static int AddTimeDuration(int timeDuration1, int timeDuration2)
        {
            return (timeDuration1.ConvertTimeDurationToMinutes() + timeDuration2.ConvertTimeDurationToMinutes()).ConvertMinutesToTimeDuration();
        }

        public static int SubtractTimeDuration(int timeDuration1, int timeDuration2)
        {
            return (timeDuration1.ConvertTimeDurationToMinutes() - timeDuration2.ConvertTimeDurationToMinutes()).ConvertMinutesToTimeDuration();
        }



        public static int ConvertDayTimeToMinutes(int time, int nextDayCount)
        {
            return nextDayCount * ServiceConstants.GeneralDayDurationInMinute + time.ConvertTimeDurationToMinutes();
        }

        public static int ConvertDayTimeToTimeDuration(int time, int nextDayCount)
        {
            return ConvertDayTimeToMinutes(time, nextDayCount).ConvertMinutesToTimeDuration();
        }





        public static int CalculateDayTimeIntervalInMinutes(int startTime, int startNextDayCount, int endTime, int endNextDayCount)
        {
            if ((startNextDayCount == endNextDayCount && startTime > endTime)
                || startNextDayCount > endNextDayCount)
            {
                return CalculateDayTimeIntervalInMinutes(endTime, endNextDayCount, startTime, startNextDayCount) * -1;
            }


            if (startNextDayCount == endNextDayCount) return endTime.ConvertTimeDurationToMinutes() - startTime.ConvertTimeDurationToMinutes();

            if (endTime < startTime)
            {
                return (endNextDayCount - startNextDayCount - 1) * ServiceConstants.GeneralDayDurationInMinute + (ServiceConstants.GeneralEndOfDayTotalMinute - startTime) + (endTime - ServiceConstants.GeneralStartOfDayTotalMinute) + 1;
            }
            return (endNextDayCount - startNextDayCount) * ServiceConstants.GeneralDayDurationInMinute + (endTime.ConvertTimeDurationToMinutes() - startTime.ConvertTimeDurationToMinutes());

        }

        public static int CalculateDayTimeIntervalInTimeDuration(int startTime, int startNextDayCount, int endTime, int endNextDayCount)
        {
            var result = CalculateDayTimeIntervalInMinutes(
                startTime
                , startNextDayCount
                , endTime
                , endNextDayCount);
            return result.ConvertMinutesToTimeDuration();

        }



        public static int GetMinutesOfOverlapBetweenTwoMinutesWithBothEdges(int firstStart, int firstEnd, int secondStart, int secondEnd)
        {
            if (firstStart > firstEnd)
                return 0;

            if (secondStart > secondEnd)
                return 0;

            var result = Math.Min(firstEnd, secondEnd) - Math.Max(firstStart, secondStart);
            return result >= 0 ? result + 1 : 0;
        }

        public static int GetMinutesOfOverlapBetweenTwoTimeDurationWithBothEdges(int firstStart, int firstEnd, int secondStart, int secondEnd)
        {
            return GetMinutesOfOverlapBetweenTwoMinutesWithBothEdges
                (firstStart.ConvertTimeDurationToMinutes()
                , firstEnd.ConvertTimeDurationToMinutes()
                , secondStart.ConvertTimeDurationToMinutes()
                , secondEnd.ConvertTimeDurationToMinutes());
        }


        public static int GetMinutesOfOverlapBetweenTwoMinutesIgnoreBothEndEdges(int firstStart, int firstEnd, int secondStart, int secondEnd)
        {
            return GetMinutesOfOverlapBetweenTwoMinutesWithBothEdges(firstStart, firstEnd - 1, secondStart, secondEnd - 1);
        }

        public static int GetMinutesOfOverlapBetweenTwotimeDurationIgnoreBothEndEdges(int firstStart, int firstEnd, int secondStart, int secondEnd)
        {

            return GetMinutesOfOverlapBetweenTwoMinutesWithBothEdges
            (firstStart.ConvertTimeDurationToMinutes()
                , firstEnd.ConvertTimeDurationToMinutes() - 1
                , secondStart.ConvertTimeDurationToMinutes()
                , secondEnd.ConvertTimeDurationToMinutes() - 1);
        }


        public static int GetMinutesOfOverlapBetweenTwoMinutesIgnoreFirstEndEdges(int firstStart, int firstEnd, int secondStart, int secondEnd)
        {
            return GetMinutesOfOverlapBetweenTwoMinutesWithBothEdges(firstStart, firstEnd - 1, secondStart, secondEnd - 1);
        }

        public static int GetMinutesOfOverlapBetweenTwotimeDurationIgnoreFirstEndEdges(int firstStart, int firstEnd, int secondStart, int secondEnd)
        {

            return GetMinutesOfOverlapBetweenTwoMinutesWithBothEdges
            (firstStart.ConvertTimeDurationToMinutes()
                , firstEnd.ConvertTimeDurationToMinutes() - 1
                , secondStart.ConvertTimeDurationToMinutes()
                , secondEnd.ConvertTimeDurationToMinutes() - 1);
        }



        #endregion


        #region Date

        /// <summary>
        /// تاریخ میلادی را دریافت کرده و به تاریخ شمسی تبدیل می کند
        /// به صورت 1394/01/01
        /// </summary>
        /// <param name="input">تاریخ میلادی</param>
        /// <returns></returns>
        public static PersianDateTime ToPersianDateTime(this DateTime input)
        {
            return new PersianDateTime(input);
        }

        /// <summary>
        /// تاریخ میلادی را دریافت کرده و به تاریخ شمسی تبدیل می کند
        /// به صورت 1394/01/01
        /// </summary>
        /// <param name="input">تاریخ میلادی</param>
        /// <returns></returns>
        public static string ToPersianDateTimeString(this DateTime input, string format)
        {
            return new PersianDateTime(input).ToString(format);
        }

        public static int ToPersianDateInt(this DateTime input)
        {
            return input.ToPersianDateTime().ToShortDateInt();
        }

        public static int ConcatYearAndMonth(int year, int month)
        {
            return year * 100 + month;
        }

        //public static int ToDateInt(this DateTime input, int yearCount)
        //{
        //	return input.ToString("yyyyMMdd").ToInt32();
        //}


        public static double ToNumericDateTime(this DateTime input)
        {
            return input.ToOADate();
        }

        public static double? ToNumericDateTime(this DateTime? input)
        {
            return input?.ToOADate();
        }

        public static DateTime FromNumericDateTime(this double input)
        {
            return DateTime.FromOADate(input);
        }

        public static DateTime? FromNumericDateTime(this double? input)
        {
            if (input.HasValue)
            {
                return DateTime.FromOADate(input.Value);
            }
            return null;
        }

        //public static long ToUnixMillisecond(this DateTime input, DateTimeKind kind)
        //{
        //	input = DateTime.SpecifyKind(input, kind);
        //	return new DateTimeOffset(input).ToUnixTimeMilliseconds();
        //}

        //public static DateTime ConvertUnitMillisecondToDateTime(this long input, DateTimeKind kind)
        //{
        //	var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(input).DateTime;
        //	return DateTime.SpecifyKind(dateTime, kind);
        //}

        //public static long ToUnixSecond(this DateTime input, DateTimeKind kind)
        //{
        //	input = DateTime.SpecifyKind(input, kind);
        //	return new DateTimeOffset(input).ToUnixTimeSeconds();
        //}

        //public static DateTime ConvertUnitSecondToDateTime(this long input, DateTimeKind kind)
        //{
        //	var dateTime = DateTimeOffset.FromUnixTimeSeconds(input).DateTime;
        //	return DateTime.SpecifyKind(dateTime, kind);
        //}

        public static DateTime ConvertDayMonthToDateTime(int monthDay)
        {
            if (monthDay <= 0)
            {
                return new DateTime(DateTime.Now.Year, 01, 01);
            }

            return new DateTime(DateTime.Now.Year, monthDay / 100, monthDay % 100);
        }


        public static DateTime? GetEndOf(DateTime? date, DateInterval interval)
        {
            if (date.HasValue)
            {
                return GetEndOf(date.Value, interval);
            }
            return null;

        }

        public static DateTime? GetStartOf(DateTime? date, DateInterval interval)
        {
            if (date.HasValue)
            {
                return GetStartOf(date.Value, interval);
            }
            return null;
        }

        public static DateTime GetEndOf(DateTime date, DateInterval interval)
        {
            switch (interval)
            {
                case DateInterval.Year:
                    return new DateTime(date.Year, 12, DateTime.DaysInMonth(date.Year, 12), 23, 59, 59);
                case DateInterval.Month:
                    return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);
                case DateInterval.Day:
                    return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
                case DateInterval.Hour:
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, 59, 59);
                case DateInterval.Minute:
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 59);
                case DateInterval.Second:
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                default:
                    return date;
            }
        }

        public static DateTime GetStartOf(DateTime date, DateInterval interval)
        {
            switch (interval)
            {
                case DateInterval.Year:
                    return new DateTime(date.Year, 1, 1, 0, 0, 0, 0);
                case DateInterval.Month:
                    return new DateTime(date.Year, date.Month, 1, 0, 0, 0, 0);
                case DateInterval.Day:
                    return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
                case DateInterval.Hour:
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0, 0);
                case DateInterval.Minute:
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0, 0);
                case DateInterval.Second:
                    return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, 0);
                default:
                    return date;
            }
        }

        public enum DateInterval
        {
            Year = 0,
            Month = 1,
            Day = 3,
            Hour = 4,
            Minute = 5,
            Second = 6,
        }

        public static long ToEpochMillisecondsTime(this DateTime date)
        {
            return new DateTimeOffset(date).ToUnixTimeMilliseconds();
        }

        public static long ToEpochSecondsTime(this DateTime date)
        {
            return new DateTimeOffset(date).ToUnixTimeSeconds();
        }

        public static DateTime ToDateTimeFromEpochMillisecondTime(this long epochMilliseconds, bool toLocalTime)
        {
            if (toLocalTime)
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(epochMilliseconds).LocalDateTime;
            }
            return DateTimeOffset.FromUnixTimeMilliseconds(epochMilliseconds).UtcDateTime;
        }

        public static DateTime ToDateTimeFromEpochSecondTime(this long epochSeconds, bool toLocalTime)
        {
            if (toLocalTime)
            {
                return DateTimeOffset.FromUnixTimeSeconds(epochSeconds).LocalDateTime;
            }
            return DateTimeOffset.FromUnixTimeSeconds(epochSeconds).UtcDateTime;
        }

        #endregion


        #region Timezone


        public static string ConvertToTimeZoneString(TimeZonesEnumeration timeZone)
        {
            var timeZoneNumber = (int)timeZone;
            if (timeZone == 0)
            {
                return "00:00";
            }

            return
                $"{(timeZoneNumber < 0 ? "-" : "+")}{Math.Abs(timeZoneNumber / 100).ToString().PadLeft(2, '0')}:{Math.Abs(timeZoneNumber % 100).ToString().PadLeft(2, '0')}";
        }


        public static int ConvertToTimeZoneTotalSecond(TimeZonesEnumeration timeZone)
        {
            var timeZoneNumber = (int)timeZone;
            return ((timeZoneNumber / 100) * 3600) + (timeZoneNumber % 100) * 60;
        }

        #endregion

    }
}
