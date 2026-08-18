using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Model.Enums;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByItem
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoiceTotalsByItem), nameof(Strings.Transactions))]
    [Guide("Shows sales invoice transactions for a specific item.")]
    [Guide("Displays all sales invoices and credit notes containing the selected item.")]
    internal sealed class SalesInvoiceTotalsByItemTransactions : TransactionViewer
    {
        [ProtoMember(1)] public DateTime From;
        [ProtoMember(2)] public DateTime To;
        [ProtoMember(3)] public Guid Item;

        protected override bool MultipleByOne()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => (x.Transaction is ManagerServer.Model.SalesInvoice || x.Transaction is ManagerServer.Model.CreditNote) && x.Item?.Key == Item && x.Date >= From && x.Date <= To && x.AccountAmount != 0m);
        }
    }
}
