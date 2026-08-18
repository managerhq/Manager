using ManagerServer.Api.Businesses.Business.Receipts;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.Receipts
{
    [ProtoContract]
    [Title(nameof(Strings.Receipt), nameof(Strings.TransactionJournal))]
    internal sealed class ReceiptTransactionJournalView : DefaultView<GetReceiptTransactionJournal>
    {
        protected override Guid? GetCustomTheme()
        {
            return null;
        }
    }
}