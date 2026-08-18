using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxTotals
{
    [ProtoContract]
    [Title(nameof(Strings.TaxTotals), nameof(Strings.Transactions))]
    [Guide("The **Tax Totals - Transactions** report provides a detailed view of all transactions that contribute to your tax totals figures.")]
    [Guide("This report breaks down transactions by *tax code* and *tax code component*, showing individual line items from sales invoices, purchase invoices, receipts, payments, and other transactions.")]
    [Guide("Use this report to verify the accuracy of your tax calculations and to identify specific transactions that make up the figures for any tax code or component.")]
    internal sealed class TaxTotalsTransactions : Summary.BaseGeneralLedgerAccountView<TaxTotalsTransactions>
    {
    }
}
