using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Receipts
{
    [ProtoContract]
    internal class GetReceiptTransactionJournal : GetTransactionJournalViewEndpoint<Receipt>
    {
    }
}
