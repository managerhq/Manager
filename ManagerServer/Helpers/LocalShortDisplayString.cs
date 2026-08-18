using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ManagerServer.Helpers
{
    public static class LocalShortDisplayString
    {
        internal static string ToLocalShortDisplayString(this DateTime date)
        {
            var shortDatePattern = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            return date.ToString(shortDatePattern);
        }

        internal static string ToLocalShortDisplayString(this DateTime? date)
        {
            if (!date.HasValue) return string.Empty;
            else return date.Value.ToLocalShortDisplayString();
        }
    }
}