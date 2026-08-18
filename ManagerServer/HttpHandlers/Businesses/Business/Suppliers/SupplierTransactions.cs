using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Suppliers
{
    [ProtoContract]
    [Title(nameof(Strings.Supplier), nameof(Strings.Transactions))]
    [Guide("This screen displays all *accounts payable* transactions for a specific supplier, providing a complete history of your financial dealings with them.")]
    [Guide("The transactions shown include *purchase invoices*, *debit notes*, *payments*, and any *journal entries* that affect the supplier's balance.")]
    [Guide("Each transaction displays the date, reference number, description, and amount. The running balance column shows the cumulative amount owed to the supplier after each transaction.")]
    [Guide("You can click on any transaction to view its full details or make edits if needed.")]
    [LinkGuide("For more information, see:", typeof(SupplierForm))]
    internal sealed class SupplierTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid Supplier;

        protected override bool MultipleByOne()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsAccountsPayable && x.Supplier?.Key == Supplier);
        }
    }
}