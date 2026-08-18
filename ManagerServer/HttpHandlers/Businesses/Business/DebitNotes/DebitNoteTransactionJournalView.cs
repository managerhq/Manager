using ManagerServer.Api.Businesses.Business.DebitNotes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.DebitNotes
{
    [ProtoContract]
    [Title(nameof(Strings.DebitNote), nameof(Strings.TransactionJournal))]
    internal sealed class DebitNoteTransactionJournalView : DefaultView<GetDebitNoteTransactionJournal>
    {
        protected override Guid? GetCustomTheme()
        {
            return null;
        }
    }
}
