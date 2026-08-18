using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using HttpFramework;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.DepreciationEntries
{
    [ProtoContract]
    [Title(nameof(Strings.DepreciationEntry))]
    [Guide("The Depreciation Entry view displays detailed information about a depreciation entry that has been recorded in your accounting system.")]
    [Guide("Depreciation entries reduce the value of your *fixed assets* over time to reflect their declining value and usage.")]
    [Guide("From this view, you can see the date, reference number, and line items that make up the depreciation entry. Click the **Edit** button to modify the entry details or correct any errors.")]
    [LinkGuide("For more information, see:", typeof(DepreciationEntryForm))]
    internal sealed class DepreciationEntryView : TransactionView<ManagerServer.Model.DepreciationEntry>
    {
        protected override IEnumerable<Tuple<string, BusinessTemplate>> GetFooterButtons()
        {
            yield return new Tuple<string, BusinessTemplate>(Strings.TransactionJournal, new DepreciationEntryTransactionJournalView() { Business = Business, Key = Key, Referrer = this.ToUrl() });
        }
    }
}