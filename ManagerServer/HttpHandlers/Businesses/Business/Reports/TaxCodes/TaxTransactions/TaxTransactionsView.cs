using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.TaxTransactions;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxTransactions
{
    [ProtoContract]
    [Title(nameof(Strings.TaxTransactions))]
    [Guide("The **Tax Transactions** report shows all transactions that have *tax codes* applied, providing a comprehensive view of your tax-related financial activities.")]
    [Guide("This report displays both sales and purchases with their associated tax amounts, organized by *tax code* for easy analysis and tax compliance.")]
    [Header("Report Content")]
    [Guide("The report includes four main columns: **Total Sales**, **Tax on Sales**, **Total Purchases**, and **Tax on Purchases**. Each transaction is grouped under its respective *tax code*, making it easy to see all transactions affected by specific tax rates.")]
    [Guide("Transaction details include the date, transaction type, and relevant customer or supplier information. Amounts are clickable, allowing you to drill down to view the original transaction.")]
    [Header("Accounting Method")]
    [Guide("The report can be generated using either *accrual basis* or *cash basis* accounting methods. This setting affects when transactions are recognized in the report - accrual basis includes all invoiced transactions, while cash basis only includes transactions when payment is received or made.")]
    [LinkGuide("For more information, see:", typeof(TaxTransactionsForm))]
    internal sealed class TaxTransactionsView : DefaultView<GetTaxTransactionsView>
    {
    }
}