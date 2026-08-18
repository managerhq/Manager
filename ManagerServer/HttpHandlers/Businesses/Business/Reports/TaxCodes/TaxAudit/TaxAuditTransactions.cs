using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TaxAudit
{
    [ProtoContract]
    [Title(nameof(Strings.TaxAudit), nameof(Strings.Transactions))]
    [Guide("The **Tax Audit - Transactions** report displays individual transactions for verifying tax code applications and calculations.")]
    [Guide("This report helps you review how tax codes have been applied to specific transactions within a selected date range and general ledger account.")]
    [Guide("Use this report to audit tax calculations, verify correct tax code assignments, and ensure compliance with tax regulations.")]
    [Guide("The report can be filtered by specific tax codes or show transactions without any tax code applied, making it easy to identify potential tax coding errors.")]
    internal sealed class TaxAuditTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid GeneralLedgerAccount;
        [ProtoMember(2)] public DateTime From;
        [ProtoMember(3)] public DateTime To;
        [ProtoMember(4)] public AccountingBasis AccountingBasis;
        [ProtoMember(5)] public Guid? TaxCode;

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business);
            if (AccountingBasis == AccountingBasis.CashBasis) generalLedger = generalLedger.AutomaticallyMatchSalesInvoices().ConvertSalesInvoicesToCashBasis2(From.AddDays(-1), To).AutomaticallyMatchPurchaseInvoices().ConvertPurchaseInvoicesToCashBasis2(From.AddDays(-1), To);

            var transactions = generalLedger.Where(x => x.GeneralLedgerAccount.Key == GeneralLedgerAccount && x.Date >= From && x.Date <= To);
            if (TaxCode.HasValue) transactions = transactions.Where(x => x.TaxCode?.Key == TaxCode.Value);
            else transactions = transactions.Where(x => x.TaxCode == null);
            return transactions;
        }
    }
}