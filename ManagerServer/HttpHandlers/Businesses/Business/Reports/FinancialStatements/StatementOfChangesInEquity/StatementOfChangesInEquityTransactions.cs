using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using HttpFramework;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.StatementOfChangesInEquity
{
    [ProtoContract]
    [Title(nameof(Strings.StatementOfChangesInEquity), nameof(Strings.Transactions))]
    [Guide("Shows detailed transactions affecting equity accounts.")]
    [Guide("Displays capital contributions, drawings, and profit allocations.")]
    internal sealed class StatementOfChangesInEquityTransactions : TransactionViewer
    {
        [ProtoMember(1)] public DateTime From;
        [ProtoMember(2)] public DateTime To;
        [ProtoMember(3)] public Guid Account;
        [ProtoMember(4)] public string Description;
        [ProtoMember(5)] public ManagerServer.Model.Enums.AccountingBasis AccountingBasis;

        protected override bool MultipleByOne()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business);
            if (AccountingBasis == ManagerServer.Model.Enums.AccountingBasis.CashBasis) transactions = transactions.AutomaticallyMatchSalesInvoices().ConvertSalesInvoicesToCashBasis2(From.AddDays(-1), To).AutomaticallyMatchPurchaseInvoices().ConvertPurchaseInvoicesToCashBasis2(From.AddDays(-1), To);

            return transactions.Where(x => x.GeneralLedgerAccount.Key == Account)
                .Where(x => x.Date >= From && x.Date <= To)
                .Where(x => x.BaseAmount != 0m)
                .Where(x => (x.TransactionLine?.GetDescriptionOrNull(x.Transaction) ?? x.Transaction?.GetDescriptionOrNull() ?? x.Transaction?.GetName()) == Description);
        }
    }
}