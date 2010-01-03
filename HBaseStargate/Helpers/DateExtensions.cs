using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HBaseStargate.Helpers
{
    public static class DateExtensions
    {
        public static long ToUnixTimestamp(this DateTime dt)
        {
            DateTime unixRef = new DateTime(1970, 1, 1, 0, 0, 0);
            return (dt.Ticks - unixRef.Ticks) / 10000000;
        }
        public static DateTime FromUnixTimestamp(long timestamp)
        {
            DateTime unixRef = new DateTime(1970, 1, 1, 0, 0, 0);
            return unixRef.AddSeconds(timestamp);
        }
    }
}
