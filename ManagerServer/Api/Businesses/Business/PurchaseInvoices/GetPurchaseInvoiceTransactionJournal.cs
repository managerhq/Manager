using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.PurchaseInvoices
{
    [ProtoContract]
    internal class GetPurchaseInvoiceTransactionJournal : GetTransactionJournalViewEndpoint<PurchaseInvoice>
    {
    }
}
