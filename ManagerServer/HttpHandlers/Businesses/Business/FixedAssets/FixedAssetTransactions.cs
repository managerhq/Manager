using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Query;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.FixedAssets
{
    [ProtoContract]
    [Title(nameof(Strings.FixedAsset), nameof(Strings.Transactions))]
    [Guide("This screen displays all transactions related to a specific *fixed asset* throughout its lifecycle.")]
    [Guide("Use this view to track the complete financial history of an individual *fixed asset* from acquisition to disposal.")]
    [Header("Types of Transactions")]
    [Guide("Transactions shown here include the initial purchase or acquisition of the asset, any improvements or additions that increase its value, and the eventual disposal when the asset is sold or written off.")]
    [Guide("Common transaction types include purchases, capital improvements, asset revaluations, and disposals.")]
    [Header("Transaction Details")]
    [Guide("Each transaction shows the date, description, and amount that affects the *fixed asset's* acquisition cost.")]
    [Guide("The transactions are displayed in chronological order, making it easy to track the complete history of the asset from acquisition to disposal.")]
    [Guide("The running balance shows the cumulative acquisition cost after each transaction.")]
    [LinkGuide("To learn more about fixed assets, see:", typeof(FixedAssetForm))]
    internal sealed class FixedAssetTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid FixedAsset;

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsControlAccountForFixedAssets && x.FixedAsset.Key == FixedAsset);
        }
    }
}
