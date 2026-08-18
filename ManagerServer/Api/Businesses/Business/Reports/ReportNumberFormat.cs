using System;
using System.Collections.Generic;
using ManagerServer.Globalization;
using ManagerServer.Helpers;

namespace ManagerServer.Api.Businesses.Business.Reports
{
    internal static class ReportNumberFormat
    {
        public static Cell Cell(decimal? value, NumberStyle style, bool wholeNumbers, Link link = null)
        {
            if (wholeNumbers && value.HasValue) value = Math.Round(value.Value, 0, MidpointRounding.AwayFromZero);
            return new Cell
            {
                Value = value,
                Text = Format(value, style, wholeNumbers),
                Link = link,
                Style = style,
            };
        }

        public static Cell Sum(IEnumerable<Cell> cells, NumberStyle style, bool wholeNumbers)
        {
            decimal? sum = null;
            if (cells != null)
            {
                foreach (var c in cells)
                {
                    if (c?.Value is decimal v)
                    {
                        sum = (sum ?? 0m) + v;
                    }
                }
            }
            return Cell(sum, style, wholeNumbers);
        }

        public static string Format(decimal? value, NumberStyle style, bool wholeNumbers)
        {
            if (value == null) return null;

            var v = value.Value;
            if (wholeNumbers) v = Math.Round(v, 0, MidpointRounding.AwayFromZero);
            if (v == 0m) return "-";

            return style switch
            {
                NumberStyle.CurrencyParentheses when v < 0m => "(" + (-v).ToNumberString() + ")",
                NumberStyle.DebitCredit when v > 0m => string.Format(Strings.XXX_Dr, v.ToNumberString()),
                NumberStyle.DebitCredit when v < 0m => string.Format(Strings.XXX_Cr, (-v).ToNumberString()),
                NumberStyle.Percentage => v.ToNumberString() + "%",
                _ => v.ToNumberString(),
            };
        }

        public static string Format(decimal? value, V2.NumberStyle style, bool wholeNumbers)
            => Format(value, ToV1Style(style), wholeNumbers);

        private static NumberStyle ToV1Style(V2.NumberStyle s) => s switch
        {
            V2.NumberStyle.Currency => NumberStyle.Currency,
            V2.NumberStyle.Quantity => NumberStyle.Quantity,
            V2.NumberStyle.Percentage => NumberStyle.Percentage,
            V2.NumberStyle.DebitCredit => NumberStyle.DebitCredit,
            V2.NumberStyle.CurrencyParentheses => NumberStyle.CurrencyParentheses,
            _ => NumberStyle.Currency,
        };
    }
}
