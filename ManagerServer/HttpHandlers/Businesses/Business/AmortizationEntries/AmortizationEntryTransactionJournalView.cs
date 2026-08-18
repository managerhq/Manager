using ManagerServer.Api.Businesses.Business.AmortizationEntries;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.AmortizationEntries
{
    [ProtoContract]
    [Title(nameof(Strings.AmortizationEntry), nameof(Strings.TransactionJournal))]
    internal sealed class AmortizationEntryTransactionJournalView : DefaultView<GetAmortizationEntryTransactionJournal>
    {
        protected override Guid? GetCustomTheme()
        {
            return null;
        }
    }
}
