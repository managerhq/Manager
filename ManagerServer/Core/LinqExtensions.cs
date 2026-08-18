using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer
{
    public static class LinqExtensions
    {
        public static decimal SafeSum(this IEnumerable<decimal> source)
        {
            try
            {
                return source.Sum();
            }
            catch (OverflowException)
            {
                return decimal.MaxValue;
            }
        }

        public static decimal SafeAdd(this decimal a, decimal b)
        {
            try
            {
                return a + b;
            }
            catch (OverflowException)
            {
                return decimal.MaxValue;
            }
        }

        public static decimal SafeMinus(this decimal a, decimal b)
        {
            try
            {
                return a - b;
            }
            catch (OverflowException)
            {
                return decimal.MaxValue;
            }
        }

        public static decimal SafeMultiply(this decimal a, decimal b)
        {
            try
            {
                return a * b;
            }
            catch (OverflowException)
            {
                return decimal.MaxValue;
            }
        }

        public static DateTime SafeAddDays(this DateTime date, int days)
        {
            try
            {
                return date.AddDays(days);
            }
            catch (ArgumentOutOfRangeException)
            {
                if (days > 0) return DateTime.MaxValue;
                return DateTime.MinValue;
            }
        }

        public static DateTime SafeAddMinutes(this DateTime date, int minutes)
        {
            try
            {
                return date.AddMinutes(minutes);
            }
            catch (ArgumentOutOfRangeException)
            {
                if (minutes > 0) return DateTime.MaxValue;
                return DateTime.MinValue;
            }
        }
    }
}