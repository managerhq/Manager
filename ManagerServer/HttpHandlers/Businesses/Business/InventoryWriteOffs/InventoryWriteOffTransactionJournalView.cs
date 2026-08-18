using ManagerServer.Api.Businesses.Business.InventoryWriteOffs;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryWriteOffs
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryWriteOff), nameof(Strings.TransactionJournal))]
    internal sealed class InventoryWriteOffTransactionJournalView : DefaultView<GetInventoryWriteOffTransactionJournal>
    {
        protected override Guid? GetCustomTheme()
        {
            return null;
        }
    }
}
