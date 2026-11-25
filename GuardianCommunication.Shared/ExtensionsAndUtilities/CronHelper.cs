using System;
using System.Globalization;

namespace GuardianCommunication.Shared.ExtensionsAndUtilities
{
    /// <summary>
    /// عبارت Cron پنج بخشی به وقت محلی سرویس: دقیقه (0-59)، ساعت (0-23)، روز ماه (1-31)، ماه (1-12)، روز هفته (0-7، 0 و 7 یکشنبه، 6 شنبه).
    /// هر بخش * یا عدد یا بازه a-b است، گام با /n (*/n، a-b/n و a/n یعنی از a تا آخر) و چند مقدار با , جدا می شوند.
    /// مثل Cron استاندارد، وقتی هیچ کدام از دو بخش روز ماه و روز هفته با * شروع نشود، روزی که یکی از آن دو را داشته باشد زمان اجراست
    /// </summary>
    public static class CronHelper
    {
        /// <summary>
        /// اولین زمان اجرای عبارت بعد از لحظه داده شده (در دقیقه کامل)؛ null یعنی تا پنج سال بعد زمانی ندارد (مثل 30 فوریه).
        /// عبارت نامعتبر FormatException می دهد
        /// </summary>
        public static DateTime? GetNextOccurrence(string cronExpression, DateTime afterDateTime)
        {
            var parts = (cronExpression ?? string.Empty).Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 5)
            {
                throw new FormatException($"Cron expression '{cronExpression}' must have 5 parts: minute hour day-of-month month day-of-week");
            }
            var minutes = ParseField(parts[0], 0, 59);
            var hours = ParseField(parts[1], 0, 23);
            var daysOfMonth = ParseField(parts[2], 1, 31);
            var months = ParseField(parts[3], 1, 12);
            var daysOfWeek = ParseField(parts[4], 0, 7);
            // 7 هم یکشنبه است
            daysOfWeek[0] = daysOfWeek[0] || daysOfWeek[7];
            var isEveryDayOfMonth = parts[2].StartsWith("*", StringComparison.Ordinal);
            var isEveryDayOfWeek = parts[4].StartsWith("*", StringComparison.Ordinal);

            var candidate = new DateTime(afterDateTime.Year, afterDateTime.Month, afterDateTime.Day, afterDateTime.Hour, afterDateTime.Minute, 0, afterDateTime.Kind)
                .AddMinutes(1);
            var lastCandidate = afterDateTime.AddYears(5);
            while (candidate <= lastCandidate)
            {
                if (!months[candidate.Month])
                {
                    candidate = new DateTime(candidate.Year, candidate.Month, 1, 0, 0, 0, candidate.Kind).AddMonths(1);
                    continue;
                }
                var isDayOfMonth = daysOfMonth[candidate.Day];
                var isDayOfWeek = daysOfWeek[(int)candidate.DayOfWeek];
                var isDay = isEveryDayOfMonth || isEveryDayOfWeek
                    ? isDayOfMonth && isDayOfWeek
                    : isDayOfMonth || isDayOfWeek;
                if (!isDay)
                {
                    candidate = candidate.Date.AddDays(1);
                    continue;
                }
                if (!hours[candidate.Hour])
                {
                    candidate = new DateTime(candidate.Year, candidate.Month, candidate.Day, candidate.Hour, 0, 0, candidate.Kind).AddHours(1);
                    continue;
                }
                if (!minutes[candidate.Minute])
                {
                    candidate = candidate.AddMinutes(1);
                    continue;
                }
                return candidate;
            }
            return null;
        }

        #region Private Methods

        /// <summary>
        /// مقدارهای مجاز یک بخش عبارت: خانه i آرایه یعنی i در این بخش هست
        /// </summary>
        private static bool[] ParseField(string field, int minValue, int maxValue)
        {
            var result = new bool[maxValue + 1];
            foreach (var item in field.Split(','))
            {
                var stepParts = item.Split('/');
                var step = 1;
                if (stepParts.Length > 2
                    || (stepParts.Length == 2
                        && (!int.TryParse(stepParts[1], NumberStyles.None, CultureInfo.InvariantCulture, out step) || step < 1)))
                {
                    throw new FormatException($"Cron part '{field}' has an invalid step");
                }
                int fromValue;
                int toValue;
                if (stepParts[0] == "*")
                {
                    fromValue = minValue;
                    toValue = maxValue;
                }
                else
                {
                    var rangeParts = stepParts[0].Split('-');
                    if (rangeParts.Length > 2
                        || !int.TryParse(rangeParts[0], NumberStyles.None, CultureInfo.InvariantCulture, out fromValue)
                        || fromValue < minValue || fromValue > maxValue)
                    {
                        throw new FormatException($"Cron part '{field}' must be *, numbers from {minValue} to {maxValue}, ranges, steps or lists");
                    }
                    toValue = fromValue;
                    if (rangeParts.Length == 2
                        && (!int.TryParse(rangeParts[1], NumberStyles.None, CultureInfo.InvariantCulture, out toValue)
                            || toValue < fromValue || toValue > maxValue))
                    {
                        throw new FormatException($"Cron part '{field}' has an invalid range");
                    }
                    // a/n یعنی از a تا آخر با گام n
                    if (rangeParts.Length == 1 && stepParts.Length == 2)
                    {
                        toValue = maxValue;
                    }
                }
                for (var value = fromValue; value <= toValue; value += step)
                {
                    result[value] = true;
                }
            }
            return result;
        }

        #endregion
    }
}
