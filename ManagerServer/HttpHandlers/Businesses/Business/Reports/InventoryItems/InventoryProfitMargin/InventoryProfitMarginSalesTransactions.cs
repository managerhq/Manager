using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryProfitMargin
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryProfitMargin), nameof(Strings.Sales), nameof(Strings.Transactions))]
    [Guide("Shows sales transactions for inventory items with profit margin analysis.")]
    [Guide("Displays revenue generated from sales of specific inventory items.")]
    internal sealed class InventoryProfitMarginSalesTransactions : TransactionViewer
    {
        [ProtoMember(1)] public DateTime From;
        [ProtoMember(2)] public DateTime To;
        [ProtoMember(3)] public Guid InventoryItem;

        protected override bool MultipleByOne()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.Date >= From && x.Date <= To)
                .Where(x => x.InventoryItem?.Key == InventoryItem)
                .Where(x => !x.GeneralLedgerAccount.IsInventoryOnHand && !x.IsTaxTransaction && x.AccountAmount != 0m)
                .Where(x => x.IsSale);
        }
    }
}