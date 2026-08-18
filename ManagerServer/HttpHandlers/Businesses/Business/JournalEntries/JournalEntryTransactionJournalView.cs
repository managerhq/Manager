using ManagerServer.Api.Businesses.Business.JournalEntries;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.JournalEntries
{
    [ProtoContract]
    [Title(nameof(Strings.JournalEntry), nameof(Strings.TransactionJournal))]
    internal sealed class JournalEntryTransactionJournalView : DefaultView<GetJournalEntryTransactionJournal>
    {
        protected override Guid? GetCustomTheme()
        {
            return null;
        }
    }
}