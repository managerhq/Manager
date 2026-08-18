using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Query;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.FixedAssets
{
    [ProtoContract]
    [Title(nameof(Strings.FixedAsset), nameof(Strings.Depreciation))]
    [Guide("This screen displays a comprehensive history of all depreciation transactions recorded for a specific fixed asset. Each transaction represents a depreciation entry that has affected the asset's value.")]
    [Header("Understanding Accumulated Depreciation")]
    [Guide("The *accumulated depreciation* balance shows the total depreciation claimed on the asset from its acquisition date to the present. This balance increases over time as depreciation is recorded, reducing the asset's *net book value*.")]
    [Guide("Accumulated depreciation represents the total wear and tear, obsolescence, or decline in value that has been recognized for accounting purposes. The difference between the asset's original cost and its accumulated depreciation equals its current *net book value*.")]
    [Header("How Depreciation Transactions Are Created")]
    [Guide("Depreciation transactions can be created in two ways:")]
    [Guide("• **Automatically** - When you process depreciation entries through the **Depreciation Entries** tab, the system calculates and records depreciation based on your asset's depreciation method and schedule.")]
    [Guide("• **Manually** - Through journal entries when you need to record depreciation adjustments or corrections outside the normal depreciation schedule.")]
    [Header("Transaction Details")]
    [Guide("The transaction list displays essential information for each depreciation entry including the date, description, and depreciation amount. Click on any transaction to view its full details and see how it affected your general ledger accounts.")]
    [Guide("Each depreciation transaction increases the *accumulated depreciation* account (a contra-asset account) and records a depreciation expense, thereby reducing the asset's *net book value* on your balance sheet.")]
    internal sealed class FixedAssetAccumulatedDepreciationTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid FixedAsset;

        protected override bool MultipleByOne()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsControlAccountForFixedAssetsAccumulatedDepreciation && x.FixedAsset.Key == FixedAsset);
        }
    }
}
