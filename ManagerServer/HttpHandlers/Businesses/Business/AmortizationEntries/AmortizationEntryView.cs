using ManagerServer.Globalization;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.AmortizationEntries
{
    [ProtoContract]
    [Title(nameof(Strings.AmortizationEntry), nameof(Strings.View))]
    [Guide("The *amortization entry* view screen displays the details of a previously created amortization entry, including the date, reference, and line items.")]
    [Guide("Access this screen by clicking the **View** button next to any amortization entry in the **Amortization Entries** tab.")]
    [Guide("From this view, you can review the complete details of the entry or click **Edit** to make changes.")]
    internal sealed class AmortizationEntryView : TransactionView<ManagerServer.Model.AmortizationEntry>
    {
        protected override IEnumerable<Tuple<string, BusinessTemplate>> GetFooterButtons()
        {
            yield return new Tuple<string, BusinessTemplate>(Strings.TransactionJournal, new AmortizationEntryTransactionJournalView() { Business = Business, Key = Key, Referrer = this.ToUrl() });
        }
    }
}