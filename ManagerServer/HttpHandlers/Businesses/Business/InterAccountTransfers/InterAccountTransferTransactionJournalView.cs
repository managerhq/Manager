using ManagerServer.Api.Businesses.Business.InterAccountTransfers;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.InterAccountTransfers
{
    [ProtoContract]
    [Title(nameof(Strings.InterAccountTransfer), nameof(Strings.TransactionJournal))]
    internal sealed class InterAccountTransferTransactionJournalView : DefaultView<GetInterAccountTransferTransactionJournal>
    {
        protected override Guid? GetCustomTheme()
        {
            return null;
        }
    }
}
