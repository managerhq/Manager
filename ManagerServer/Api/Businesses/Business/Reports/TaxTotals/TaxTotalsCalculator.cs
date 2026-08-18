using ManagerServer.Model.Enums;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxTotals
{
    internal static class TaxTotalsCalculator
    {
        public sealed class TaxCodeTotals
        {
            public Model.TaxCode TaxCode { get; set; }
            public decimal TaxExclusiveTotal { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal TaxInclusiveTotal => TaxExclusiveTotal + TaxAmount;
            public ComponentTotals[] Components { get; set; }
        }

        public sealed class ComponentTotals
        {
            public string Name { get; set; }
            public decimal TaxAmount { get; set; }
        }

        public static TaxCodeTotals[] Calculate(string business, Model.TaxTotals report)
        {
            var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(business);
            if (report.AccountingMethod == AccountingBasis.CashBasis) generalLedger = generalLedger.AutomaticallyMatchSalesInvoices().ConvertSalesInvoicesToCashBasis2(report.FromDate.AddDays(-1), report.ToDate).AutomaticallyMatchPurchaseInvoices().ConvertPurchaseInvoicesToCashBasis2(report.FromDate.AddDays(-1), report.ToDate);
            var transactions = generalLedger.Where(x => x.TaxCode != null && x.Date >= report.FromDate && x.Date <= report.ToDate);
            if (report.Division.HasValue) transactions = transactions.Where(x => x.Division?.Key == report.Division.Value);

            return transactions
                .GroupBy(x => x.TaxCode)
                .OrderBy(x => x.Key.Name)
                .Select(e =>
                {
                    // Sales post as credits (negative), purchases as debits (positive); negating the sum
                    // yields net figures for the period (sales minus purchases).
                    var components = e.Where(x => x.IsTaxTransaction)
                        .GroupBy(x => x.TaxComponent ?? string.Empty)
                        .OrderBy(x => x.Key)
                        .Select(c => new ComponentTotals { Name = c.Key, TaxAmount = c.Sum(x => x.BaseAmount) * -1m })
                        .ToArray();

                    return new TaxCodeTotals
                    {
                        TaxCode = e.Key,
                        TaxExclusiveTotal = e.Where(x => !x.IsTaxTransaction).Sum(x => x.BaseAmount) * -1m,
                        TaxAmount = components.Sum(x => x.TaxAmount),
                        Components = components.Length > 1 ? components : null,
                    };
                })
                .ToArray();
        }
    }
}
