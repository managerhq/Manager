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

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxablePurchasesPerSupplier
{
    [ProtoContract]
    [Title(nameof(Strings.TaxablePurchasesPerSupplier), nameof(Strings.Transactions))]
    [Guide("This report shows all taxable purchase transactions for a specific supplier within a selected date range.")]
    [Guide("It includes *purchase invoices* and *debit notes* that contain tax amounts, helping you track taxable purchases from individual suppliers.")]
    [Header("What This Report Shows")]
    [Guide("The report displays a detailed list of transactions with the following information:")]
    [Guide("• Transaction date and reference number")]
    [Guide("• Document type (*purchase invoice* or *debit note*)")]
    [Guide("• Description of the purchase")]
    [Guide("• Tax amounts applied to each transaction")]
    [Guide("• Running totals of taxable purchases")]
    [Header("Using This Report")]
    [Guide("This report is useful for:")]
    [Guide("• Reviewing all taxable purchases from a specific supplier")]
    [Guide("• Verifying tax amounts charged by suppliers")]
    [Guide("• Preparing supplier-specific tax documentation")]
    [Guide("• Reconciling supplier statements with your tax records")]
    [Header("Report Options")]
    [Guide("You can filter this report by:")]
    [Guide("• **Date range** - Select the period you want to analyze")]
    [Guide("• **Accounting basis** - Choose between *cash basis* (when paid) or *accrual basis* (when invoiced)")]
    internal sealed class TaxablePurchasesPerSupplierTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid TaxCode;
        [ProtoMember(2)] public Guid Supplier;
        [ProtoMember(3)] public DateTime From;
        [ProtoMember(4)] public DateTime To;
        [ProtoMember(5)] public AccountingBasis AccountingBasis;

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business);
            if (AccountingBasis == AccountingBasis.CashBasis) transactions = transactions.AutomaticallyMatchPurchaseInvoices(new Guid[] { Supplier }).ConvertPurchaseInvoicesToCashBasis2(From.AddDays(-1), To);
            return transactions.Where(x => x.IsTaxTransaction && (x.Transaction is ManagerServer.Model.PurchaseInvoice || x.Transaction is ManagerServer.Model.DebitNote) && x.Date >= From && x.Date <= To && x.Supplier?.Key == Supplier && x.TaxCode?.Key == TaxCode);
        }
    }
}