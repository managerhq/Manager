using ManagerServer.Api.Businesses.Business.SalesInvoices;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoice), nameof(Strings.TransactionJournal))]
    internal sealed class SalesInvoiceTransactionJournalView : DefaultView<GetSalesInvoiceTransactionJournal>
    {
        protected override Guid? GetCustomTheme()
        {
            return null;
        }
    }
}
