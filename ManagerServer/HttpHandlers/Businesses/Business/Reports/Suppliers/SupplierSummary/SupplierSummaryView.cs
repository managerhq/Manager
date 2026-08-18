using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.SupplierSummary;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SupplierSummary
{
    [ProtoContract]
    [Title(nameof(Strings.SupplierSummary))]
    [Guide("The **Supplier Summary** report provides a comprehensive overview of all supplier activity during a specified period.")]
    [Guide("This report helps you track amounts owed to suppliers and monitor payment patterns across your supplier base.")]
    [Header("Report Components")]
    [Guide("The report displays the following information for each supplier:")]
    [Guide("• *Opening balance* - The amount owed at the beginning of the reporting period")]
    [Guide("• *Invoices* - Total value of purchase invoices received during the period")]
    [Guide("• *Payments* - Total payments made to suppliers")]
    [Guide("• *Refunds* - Any refunds received from suppliers")]
    [Guide("• *Debit notes* - Adjustments reducing amounts owed to suppliers")]
    [Guide("• *Journal entries* - Manual adjustments affecting supplier balances")]
    [Guide("• *Closing balance* - The final amount owed at the end of the reporting period")]
    [Header("Currency Handling")]
    [Guide("When working with multiple currencies, the report groups suppliers by currency to provide accurate totals.")]
    [Guide("Each currency group displays subtotals, making it easy to see your total liability in each currency.")]
    [LinkGuide("To customize this report, see:", typeof(SupplierSummaryForm))]
    internal sealed class SupplierSummaryView : DefaultView<GetSupplierSummaryView>
    {
    }
}