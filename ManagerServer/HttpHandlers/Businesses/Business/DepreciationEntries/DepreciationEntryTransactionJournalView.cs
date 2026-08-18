using ManagerServer.Api.Businesses.Business.DepreciationEntries;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.DepreciationEntries
{
    [ProtoContract]
    [Title(nameof(Strings.DepreciationEntry), nameof(Strings.TransactionJournal))]
    internal sealed class DepreciationEntryTransactionJournalView : DefaultView<GetDepreciationEntryTransactionJournal>
    {
        protected override Guid? GetCustomTheme()
        {
            return null;
        }
    }
}
