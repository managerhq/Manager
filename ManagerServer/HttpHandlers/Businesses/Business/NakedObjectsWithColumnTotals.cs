using ManagerServer.Model.Enums;
using ManagerServer.Helpers;
using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithColumnTotals : NakedObjectsWithBanners
    {
        public sealed class SumAttribute : Attribute { }

        protected override void OnColumnFooterCell(Column column, Array rows)
        {
            if (!column.Attributes.OfType<SumAttribute>().Any()) return;

            try
            {
                if (column is Column<decimal> decimalColumn)
                {
                    var total = rows.Cast<object>().Sum(x => decimalColumn.GetRowValue(x));
                    Write(total.ToNumberString());
                    return;
                }

                if (column is Column<Tuple<decimal, DebitCredit>> decimalDebitCreditColumn)
                {
                    var total = rows.Cast<object>()
                        .Select(x => decimalDebitCreditColumn.GetRowValue(x))
                        .Where(x => x != null)
                        .Select(x => x.Item2 == DebitCredit.Debit ? x.Item1 : x.Item1 * -1m)
                        .Sum(x => x);
                    Write(total.ToCurrencyStringAsDrCr(null, CurrencySymbol.Short));
                    return;
                }

                if (column is Column<Tuple<decimal, BusinessTemplate>> hyperlinkDecimalColumn)
                {
                    var total = rows.Cast<object>().Sum(x => hyperlinkDecimalColumn.GetRowValue(x)?.Item1);
                    Write(total.ToNumberString());
                    return;
                }

                if (column is Column<int> intColumn)
                {
                    var total = rows.Cast<object>().Sum(x => intColumn.GetRowValue(x));
                    Write(total.ToString());
                    return;
                }

                if (column is Column<Tuple<int, BusinessTemplate>> hyperlinkIntColumn)
                {
                    var total = rows.Cast<object>().Sum(x => hyperlinkIntColumn.GetRowValue(x)?.Item1);
                    Write(total.ToString());
                    return;
                }

                if (column is Column<decimal?> nullableDecimalColumn)
                {
                    var total = rows.Cast<object>().Select(x => nullableDecimalColumn.GetRowValue(x)).Where(x => x != null).Sum(x => x.Value);
                    Write(total.ToNumberString());
                    return;
                }

                if (column is Column<Tuple<decimal, ManagerServer.Model.Currency>> currencyAmountColumn)
                {
                    var totals = rows.Cast<object>().Select(x => currencyAmountColumn.GetRowValue(x)).Where(x => x != null).GroupBy(x => x.Item2).Select(x => new Tuple<decimal, ManagerServer.Model.Currency>(x.Sum(x => x.Item1), x.Key)).OrderByDescending(x => x.Item1).ToArray();
                    foreach (var e in totals)
                    {
                        using (Div()) Write(e.Item1.ToCurrencyString(e.Item2, CurrencySymbol.Short));
                    }
                    return;
                }

                if (column is Column<Tuple<DebitCreditAmount, ManagerServer.Model.Currency>> debitCreditCurrencyAmountColumn)
                {
                    var totals = rows.Cast<object>().Select(x => debitCreditCurrencyAmountColumn.GetRowValue(x)).Where(x => x != null).GroupBy(x => x.Item2).Select(x => new Tuple<decimal, ManagerServer.Model.Currency>(x.Sum(x => x.Item1.Value), x.Key)).OrderByDescending(x => x.Item1).ToArray();
                    foreach (var e in totals)
                    {
                        using (Div()) Write(e.Item1.ToCurrencyStringAsDrCr(e.Item2, CurrencySymbol.Short));
                    }
                    return;
                }

                if (column is Column<Tuple<decimal, ManagerServer.Model.Currency, BusinessTemplate>> hyperlinkCurrencyAmountColumn)
                {
                    var totals = rows.Cast<object>().Select(x => hyperlinkCurrencyAmountColumn.GetRowValue(x)).Where(x => x != null).GroupBy(x => x.Item2).Select(x => new Tuple<decimal, ManagerServer.Model.Currency>(x.Sum(x => x.Item1), x.Key)).OrderByDescending(x => x.Item1).ToArray();
                    foreach (var e in totals)
                    {
                        using (Div()) Write(e.Item1.ToCurrencyString(e.Item2, CurrencySymbol.Short));
                    }
                    return;
                }

                if (column is Column<Tuple<DebitCreditAmount, ManagerServer.Model.Currency, BusinessTemplate>> hyperlinkCurrencyDebitCreditAmountColumn)
                {
                    var totals = rows.Cast<object>().Select(x => hyperlinkCurrencyDebitCreditAmountColumn.GetRowValue(x)).Where(x => x != null).GroupBy(x => x.Item2).Select(x => new Tuple<decimal, ManagerServer.Model.Currency>(x.Sum(x => x.Item1.Value), x.Key)).OrderByDescending(x => x.Item1).ToArray();
                    foreach (var e in totals)
                    {
                        using (Div()) Write(e.Item1.ToCurrencyStringAsDrCr(e.Item2, CurrencySymbol.Short));
                    }
                    return;
                }
            }
            catch (OverflowException)
            {
                // If total is too large, then don't bother
            }
        }
    }
}
