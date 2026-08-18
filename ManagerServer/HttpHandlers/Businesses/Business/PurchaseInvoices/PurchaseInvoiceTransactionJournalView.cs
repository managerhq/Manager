using ManagerServer.Api.Businesses.Business.PurchaseInvoices;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.PurchaseInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.PurchaseInvoice), nameof(Strings.TransactionJournal))]
    internal sealed class PurchaseInvoiceTransactionJournalView : DefaultView<GetPurchaseInvoiceTransactionJournal>
    {
        protected override Guid? GetCustomTheme()
        {
            return null;
        }
    }
}
