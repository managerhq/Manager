using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByCustomer
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoiceTotalsByCustomer), nameof(Strings.Transactions))]
    [Guide("Shows sales invoice transactions for a specific customer.")]
    [Guide("Displays all sales invoices and credit notes for the selected customer and period.")]
    internal sealed class SalesInvoiceTotalsByCustomerTransactions : TransactionViewer
    {
        [ProtoMember(1)] public DateTime From;
        [ProtoMember(2)] public DateTime To;
        [ProtoMember(3)] public Guid Customer;

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsAccountsReceivable && (x.Transaction is ManagerServer.Model.SalesInvoice || x.Transaction is ManagerServer.Model.CreditNote) && x.Customer.Key == Customer && x.Date >= From && x.Date <= To);
        }
    }
}
