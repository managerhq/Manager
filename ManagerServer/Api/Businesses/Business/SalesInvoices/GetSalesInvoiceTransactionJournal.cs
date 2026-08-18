using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.SalesInvoices
{
    [ProtoContract]
    internal class GetSalesInvoiceTransactionJournal : GetTransactionJournalViewEndpoint<SalesInvoice>
    {
    }
}
