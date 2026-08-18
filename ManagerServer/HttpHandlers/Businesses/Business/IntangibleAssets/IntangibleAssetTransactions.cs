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
    [Title(nameof(Strings.IntangibleAsset), nameof(Strings.AcquisitionCost))]
    [Guide("The **Acquisition Cost** screen displays all transactions that affect the acquisition cost of an intangible asset.")]
    [Guide("This comprehensive view provides a complete transaction history for tracking how your intangible asset's value has changed over time.")]
    [Header("Types of Transactions")]
    [Guide("The following types of transactions appear in this view:")]
    [Guide("• **Initial purchase** - The original cost when you first acquire the intangible asset")]
    [Guide("• **Additional costs** - Any subsequent expenditures that increase the asset's value or extend its useful life")]
    [Guide("• **Disposal transactions** - Entries recorded when the asset is sold, written off, or otherwise removed from your books")]
    [Header("Transaction Details")]
    [Guide("Each transaction displays essential information to help you understand the changes to your asset:")]
    [Guide("• **Date** - When the transaction occurred")]
    [Guide("• **Description** - Details about the nature of the transaction")]
    [Guide("• **Amount** - The monetary value of the transaction")]
    [Guide("• **Running balance** - The cumulative *acquisition cost* after each transaction, helping you track the total investment in the asset at any point in time")]
    internal sealed class IntangibleAssetTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid IntangibleAsset;

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsControlAccountForIntangibleAssets && x.IntangibleAsset.Key == IntangibleAsset);
        }
    }
}
