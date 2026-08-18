using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using ManagerServer.Globalization;
using System.Threading;

namespace ManagerServer.Helpers
{
    public enum CurrencySymbol
    {
        None,
        Short,
        Long
    }

    public static class DecimalExtensions
    {
        internal static string ToCurrencyString(this decimal value, ManagerServer.Model.Currency currency, CurrencySymbol currencySymbol)
        {
            var prefix = currency?.GetPrefix()+ " ";
            var suffix = " " + currency?.GetSuffix();

            if (string.IsNullOrWhiteSpace(prefix) && string.IsNullOrWhiteSpace(suffix))
            {
                suffix = " " + currency?.GetCode();
            }

            if (currencySymbol == CurrencySymbol.None)
            {
                prefix = string.Empty;
                suffix = string.Empty;
            }

            if (value < 0)
            {
                value = value * -1;
                prefix = "- " + prefix;
            }

            value = value / 1.000000000000000000000000000000000m;

            var numberDecimalDigits = 2;
            if (currency != null) numberDecimalDigits = currency.GetDecimalPlaces();
            var nums = Decimal.GetBits(value);
            var decimals = BitConverter.GetBytes(nums[3])[2];
            if (decimals > numberDecimalDigits)
            {
                return prefix +value.ToString("N" + decimals, Thread.CurrentThread.CurrentCulture.NumberFormat) + suffix;
            }
            else
            {
                return prefix + value.ToString("N" + numberDecimalDigits, Thread.CurrentThread.CurrentCulture.NumberFormat) + suffix;
            }
        }

        internal static decimal TrimTrailingZeroes(this decimal value)
        {
            return value / 1.000000000000000000000000000000000m;
        }

        internal static string ToNumberString(this decimal value)
        {            
            var nums = Decimal.GetBits(value);
            var decimals = BitConverter.GetBytes(nums[3])[2];
            return value.ToString("N" + decimals, Thread.CurrentThread.CurrentCulture.NumberFormat);
        }

        internal static string ToCurrencyStringWithParentheses(this decimal value, ManagerServer.Model.Currency currency, CurrencySymbol currencySymbol)
        {
            if (value < 0m) return "(" + (value * -1).ToCurrencyString(currency: currency, currencySymbol: currencySymbol) + ")";
            else return value.ToCurrencyString(currency: currency, currencySymbol: currencySymbol);
        }

        internal static string ToCurrencyStringAsDrCr(this decimal value, ManagerServer.Model.Currency currency, CurrencySymbol currencySymbol)
        {
            if (value > 0) return string.Format(Strings.XXX_Dr, value.ToCurrencyString(currency: currency, currencySymbol: currencySymbol));
            else if (value < 0) return string.Format(Strings.XXX_Cr, (value * -1).ToCurrencyString(currency: currency, currencySymbol: currencySymbol));
            else return value.ToCurrencyString(currency: currency, currencySymbol: currencySymbol);
        }

        internal static string ToCurrencyString(this decimal? value, ManagerServer.Model.Currency currency, CurrencySymbol currencySymbol)
        {
            if (!value.HasValue) return null;
            return value.Value.ToCurrencyString(currency: currency, currencySymbol: currencySymbol);
        }

        internal static string ToNumberString(this decimal? value)
        {
            if (!value.HasValue) return null;
            return value.Value.ToNumberString();
        }
    }
}