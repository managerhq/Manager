using ManagerServer.Api.Businesses.Business.CreditNotes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.CreditNotes
{
    [ProtoContract]
    [Title(nameof(Strings.CreditNote), nameof(Strings.TransactionJournal))]
    internal sealed class CreditNoteTransactionJournalView : DefaultView<GetCreditNoteTransactionJournal>
    {
        protected override Guid? GetCustomTheme()
        {
            return null;
        }
    }
}
