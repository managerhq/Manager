using System;
using System.Linq;
using System.Collections.Generic;
using ManagerServer.Query;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.Customers
{
    [ProtoContract]
    [Title(nameof(Strings.Customer), nameof(Strings.Transactions))]
    [Guide("This screen displays all *accounts receivable* transactions for a specific customer, providing a complete history of your financial interactions with them.")]
    [Guide("The transactions shown include *sales invoices*, *credit notes*, *receipts*, and *journal entries* that affect the customer's balance.")]
    [Guide("Each transaction displays the date, reference number, description, and the amount owed or paid. The running balance column shows the customer's balance after each transaction.")]
    [Guide("You can use this screen to quickly review a customer's payment history, identify outstanding invoices, and track how their balance has changed over time.")]
    [LinkGuide("For more information, see:", typeof(CustomerForm))]
    internal sealed class CustomerTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid Customer;

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsAccountsReceivable && x.Customer?.Key == Customer);
        }
    }
}
