using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Query;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.IntangibleAssets
{
    [ProtoContract]
    [Title(nameof(Strings.IntangibleAsset), nameof(Strings.Amortization))]
    [Guide("This screen displays all *accumulated amortization* transactions for a specific intangible asset.")]
    [Guide("Amortization is the process of systematically allocating the cost of an intangible asset over its useful life, similar to how depreciation works for fixed assets.")]
    [Header("Understanding the Transactions")]
    [Guide("The transactions shown here represent the periodic amortization entries that have been recorded for this intangible asset.")]
    [Guide("Each entry increases the *accumulated amortization* balance, which in turn reduces the *net book value* of the intangible asset.")]
    [Guide("These entries are typically created through amortization entries or journal entries that affect the intangible asset's accumulated amortization account.")]
    [Header("Reading the Information")]
    [Guide("The *running balance* column shows the total accumulated amortization at any point in time.")]
    [Guide("You can use this information to track how much of the intangible asset's cost has been amortized and how much remains to be amortized over its remaining useful life.")]
    internal sealed class IntangibleAssetAccumulatedAmortizationTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid IntangibleAsset;

        protected override bool MultipleByOne()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsControlAccountForIntangibleAssetsAccumulatedAmortization && x.IntangibleAsset.Key == IntangibleAsset);
        }
    }
}
