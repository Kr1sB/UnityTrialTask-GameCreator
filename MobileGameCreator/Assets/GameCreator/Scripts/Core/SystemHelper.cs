using System;
using System.Collections.Generic;

//TODO: Find a better solution for timezones on Android!

namespace GameCreator
{
    public static class SystemHelper
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        private static Dictionary<string, TimeZoneInfo> timeZones = new Dictionary<string, TimeZoneInfo>();

        static SystemHelper()
        {
            AddTimeZone("America/New_York", -5);
            AddTimeZone("Europe/Berlin", 1);
            AddTimeZone("Europe/Moscow", 3);
            AddTimeZone("Asia/Hong_Kong", 8);
            AddTimeZone("Australia/Sydney", 11);
            AddTimeZone("Europe/London", 0);
        }

        private static TimeZoneInfo AddTimeZone(string id, int utcOffset)
        {
            TimeZoneInfo info = TimeZoneInfo.CreateCustomTimeZone(id, TimeSpan.FromHours(utcOffset), id, id);
            timeZones.Add(id, info);

            return info;
        }

        public static TimeZoneInfo GetTimeZoneInfoById(string id)
        {

            if (timeZones.TryGetValue(id, out TimeZoneInfo info))
                return info;

            return null;
        }
#else

        public static TimeZoneInfo GetTimeZoneInfoById(string id) =>
            TimeZoneInfo.FindSystemTimeZoneById(id);

#endif
    }
}