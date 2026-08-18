using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.InterAccountTransfers
{
    [ProtoContract]
    internal class GetInterAccountTransferTransactionJournal : GetTransactionJournalViewEndpoint<InterAccountTransfer>
    {
    }
}
