using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Model.Enums;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxReconciliation
{
    [ProtoContract]
    [Title(nameof(Strings.TaxReconciliation), nameof(Strings.Transactions))]
    [Guide("The **Tax Reconciliation Transactions** report provides a detailed view of all tax-related transactions within a specified period. This report helps you reconcile your tax accounts by showing exactly how tax was collected, paid, and adjusted.")]
    [Guide("Use this report to verify that your tax records match with your tax authority's requirements and to identify any discrepancies that need correction.")]
    [Header("Transaction Categories")]
    [Guide("The report can display five different categories of transactions:")]
    [Guide("• **Payments** - Shows non-tax payments made from the selected tax account")]
    [Guide("• **Receipts** - Shows non-tax receipts received into the selected tax account")]
    [Guide("• **Adjustments** - Shows journal entries and other adjustments that affect the tax account balance")]
    [Guide("• **Tax Paid** - Shows tax amounts paid to suppliers on purchases")]
    [Guide("• **Tax Collected** - Shows tax amounts collected from customers on sales")]
    [Header("Using the Report")]
    [Guide("Select the appropriate date range and *accounting basis* to match your tax reporting requirements. Choose which transaction categories to include based on what you need to reconcile.")]
    [Guide("The report will display transaction details including dates, descriptions, and amounts, making it easy to trace each entry back to its source document.")]
    internal sealed class TaxReconciliationTransactions : TransactionViewer
    {
        [ProtoMember(1)] public DateTime From;
        [ProtoMember(2)] public DateTime To;
        [ProtoMember(3)] public AccountingBasis AccountingBasis;
        [ProtoMember(4)] public Guid GeneralLedgerAccount;
        [ProtoMember(5)] public bool Payments;
        [ProtoMember(6)] public bool Receipts;
        [ProtoMember(7)] public bool Adjustments;
        [ProtoMember(8)] public bool TaxPaid;
        [ProtoMember(9)] public bool TaxCollected;

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business);
            if (AccountingBasis == AccountingBasis.CashBasis) generalLedger = generalLedger.AutomaticallyMatchSalesInvoices().ConvertSalesInvoicesToCashBasis2(From.AddDays(-1), To).AutomaticallyMatchPurchaseInvoices().ConvertPurchaseInvoicesToCashBasis2(From.AddDays(-1), To);

            var transactions = generalLedger.Where(x => x.GeneralLedgerAccount.Key == GeneralLedgerAccount && x.Date >= From && x.Date <= To);
            if (Payments) return transactions.Where(x => !x.IsTaxTransaction && x.Payment != null);
            if (Receipts) return transactions.Where(x => !x.IsTaxTransaction && x.Receipt != null);
            if (Adjustments) return transactions.Where(x => !x.IsTaxTransaction && x.Receipt == null && x.Payment == null);
            if (TaxPaid) return transactions.Where(x => x.IsTaxTransaction && !x.IsSale);
            if (TaxCollected) return transactions.Where(x => x.IsTaxTransaction && x.IsSale);
            return null;
        }
    }
}
