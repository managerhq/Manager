using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using HttpFramework;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Query;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxableSalesPerCustomer
{
    [ProtoContract]
    [Title(nameof(Strings.TaxableSalesPerCustomer), nameof(Strings.Transactions))]
    [Guide("This report shows all taxable sales transactions for a specific customer within a selected date range.")]
    [Guide("The report includes both *sales invoices* and *credit notes* that have tax codes applied to them, allowing you to review all tax-related sales activity for the customer.")]
    [Guide("Each transaction displays the date, reference number, description, and the tax amount calculated based on the applied *tax code*.")]
    [Guide("Use this report to verify tax calculations for individual customers or to prepare customer-specific tax documentation.")]
    internal sealed class TaxableSalesPerCustomerTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid TaxCode;
        [ProtoMember(2)] public Guid Customer;
        [ProtoMember(3)] public DateTime From;
        [ProtoMember(4)] public DateTime To;
        [ProtoMember(5)] public AccountingBasis AccountingBasis;

        protected override bool MultipleByOne()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business);
            if (AccountingBasis == AccountingBasis.CashBasis) transactions = transactions.AutomaticallyMatchSalesInvoices(new Guid[] { Customer }).ConvertSalesInvoicesToCashBasis2(From.AddDays(-1), To);
            return transactions.Where(x => x.IsTaxTransaction && (x.Transaction is ManagerServer.Model.SalesInvoice || x.Transaction is ManagerServer.Model.CreditNote) && x.Date >= From && x.Date <= To && x.Customer?.Key == Customer && x.TaxCode?.Key == TaxCode);
        }
    }
}