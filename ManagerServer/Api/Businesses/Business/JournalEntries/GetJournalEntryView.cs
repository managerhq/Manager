using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.JournalEntries
{
    [ProtoContract]
    internal sealed class GetJournalEntryView : GetTransactionView<Model.JournalEntry>
    {
        protected override TransactionView GetViewData(Model.JournalEntry o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.JournalEntry;
            viewData.reference = o.Reference;
            viewData.description = o.Narration;

            viewData.fields.Add(new TransactionView.Field { label = Strings.Date, text = o.Date.ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });

            viewData.table = BuildTable(o, showTaxAmountOnLineItems: false);
            viewData.table.totals = new List<TransactionView.Total>();

            if (o.GetGeneralLedgerTransactions(Database).Where(x => x.TransactionLine != null).Sum(x => x.TransactionAmount) != 0m)
            {
                viewData.emphasis = new TransactionView.Emphasis { negative = true, text = Strings.Unbalanced };
            }

            return viewData;
        }
    }
}
