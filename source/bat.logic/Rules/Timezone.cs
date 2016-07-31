using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Rules
{
    public class Timezone
    {
        private static TimeZoneInfo tzidGB
        {
            get
            {
                //// Get timezone id's here: http://stackoverflow.com/questions/5996320/net-timezoneinfo-from-olson-time-zone
                return TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            }
        }

        public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzidGB);

        public static DateTime Today => new DateTime(Now.Year, Now.Month, Now.Day);

        public static DateTime ConvertToUTC(DateTime date)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                Convert.ToDateTime(date.ToString("dd MMM yyyy hh:mm:ss tt")), "GMT Standard Time", "UTC");
        }

        public static DateTime? ConvertToUTC(DateTime? date) => date.HasValue ? ConvertToUTC(date.Value) : (DateTime?)null;

        public static DateTime ConvertFromUTC(DateTime date)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(date, tzidGB);
        }

        public static DateTime? ConvertFromUTC(DateTime? date) => date.HasValue ? ConvertFromUTC(date.Value) : (DateTime?)null;
    }
}
