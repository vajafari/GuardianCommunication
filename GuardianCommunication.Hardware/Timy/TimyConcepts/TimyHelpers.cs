using System;

namespace GuardianCommunication.Hardware.Timy.TimyConcepts
{
    internal static class TimyHelpers
    {

        //public static byte[] ConvertStringToBytes(string str)
        //{
        //    return Convert.FromBase64String(str);
        //    return Encoding.UTF8.GetBytes(str);
        //}

        //public static string ConvertBytesToString(byte[] bytes)
        //{
        //    return Convert.ToBase64String(bytes);
        //    return Encoding.UTF8.GetString(bytes);
        //}

        public static int GetTimeStamp(DateTime dt)
        {
            var dateStart = new DateTime(2000, 1, 1, 0, 0, 0);
            var timeStamp = Convert.ToInt32((dt - dateStart).TotalSeconds);
            return timeStamp;
        }

        public static DateTime GetDateTime(int timeStamp)
        {
            var dtStart = new DateTime(2000, 1, 1, 0, 0, 0);
            var lTime = ((long)timeStamp * 10000000);
            var toNow = new TimeSpan(lTime);
            var targetDt = dtStart.Add(toNow);
            return targetDt;
        }

    }
}
