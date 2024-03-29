﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Rules
{
    public class Timezone
    {
        //// Get timezone id's here: http://stackoverflow.com/questions/5996320/net-timezoneinfo-from-olson-time-zone
        private static string GMT = "GMT Standard Time";

        public static DateTime Now => ConvertFromUTC(DateTime.UtcNow);

        public static DateTime Today => new DateTime(Now.Year, Now.Month, Now.Day);

        public static DateTime ConvertFromUTC(DateTime date) =>
            TimeZoneInfo.ConvertTimeFromUtc(new DateTime(date.Ticks), TimeZoneInfo.FindSystemTimeZoneById(GMT));

        public static DateTime ConvertToUTC(DateTime date) =>
            TimeZoneInfo.ConvertTimeBySystemTimeZoneId(new DateTime(date.Ticks), GMT, "UTC");

        public static DateTime? ConvertFromUTC(DateTime? date) => date.HasValue ? ConvertFromUTC(date.Value) : (DateTime?)null;

        public static DateTime? ConvertToUTC(DateTime? date) => date.HasValue ? ConvertToUTC(date.Value) : (DateTime?)null;
    }
}
