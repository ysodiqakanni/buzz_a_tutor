using System;
using System.Collections.Generic;
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
            switch (date.Kind)
            {
                case DateTimeKind.Local:
                    return date.ToUniversalTime();

                case DateTimeKind.Utc:
                    return date;

                case DateTimeKind.Unspecified:
                    break;
            }

            return TimeZoneInfo.ConvertTimeToUtc(date, tzidGB);
        }

        public static DateTime? ConvertToUTC(DateTime? date) => date.HasValue ? ConvertToUTC(date.Value) : (DateTime?)null;

        public static DateTime ConvertFromUTC(DateTime date)
        {
            switch (date.Kind)
            {
                case DateTimeKind.Local:
                    date = date.ToUniversalTime();
                    break;

                case DateTimeKind.Utc:
                    break;

                case DateTimeKind.Unspecified:
                    break;
            }

            return TimeZoneInfo.ConvertTimeFromUtc(date, tzidGB);
        }

        public static DateTime? ConvertFromUTC(DateTime? date) => date.HasValue ? ConvertFromUTC(date.Value) : (DateTime?)null;
    }
}
