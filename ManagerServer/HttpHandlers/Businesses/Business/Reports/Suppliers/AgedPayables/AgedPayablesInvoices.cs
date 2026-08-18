using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Helpers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.AgedPayables
{
    [ProtoContract]
    [Title(nameof(Strings.AgedPayables), nameof(Strings.Invoices))]
    [Guide("Shows the unpaid purchase invoices contributing to a particular Aged Payables figure.")]
    [Guide("The list is filtered to a specific supplier and aging period as of the report date.")]
    internal sealed class AgedPayablesInvoices : TransactionViewer
    {
        [ProtoMember(1)] public Guid Supplier;
        [ProtoMember(2)] public DateTime Date;
        [ProtoMember(3)] public int Bucket;

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .AutomaticallyMatchPurchaseInvoices(new[] { Supplier })
                .Where(x => x.Date <= Date && x.GeneralLedgerAccount.IsAccountsPayable && x.Supplier?.Key == Supplier && x.PurchaseInvoice != null)
                .ToArray();

            var matchingInvoices = transactions
                .GroupBy(x => x.PurchaseInvoice)
                .Where(g => g.Sum(y => y.AccountAmount) != 0m)
                .Select(g => g.Key)
                .Where(InBucket)
                .Select(x => x.Key)
                .ToHashSet();

            return transactions.Where(x => matchingInvoices.Contains(x.PurchaseInvoice.Key));
        }

        private bool InBucket(ManagerServer.Model.PurchaseInvoice purchaseInvoice)
        {
            var dueDate = purchaseInvoice.GetDueDate();
            var days30 = Date.SafeAddDays(-30);
            var days60 = Date.SafeAddDays(-60);
            var days90 = Date.SafeAddDays(-90);

            return Bucket switch
            {
                0 => dueDate >= Date,
                1 => dueDate < Date && dueDate >= days30,
                2 => dueDate < days30 && dueDate >= days60,
                3 => dueDate < days60 && dueDate >= days90,
                4 => dueDate < days90,
                _ => false,
            };
        }
    }
}
