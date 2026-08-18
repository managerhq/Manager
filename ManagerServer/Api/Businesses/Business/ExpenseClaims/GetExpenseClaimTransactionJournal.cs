using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.ExpenseClaims
{
    [ProtoContract]
    internal class GetExpenseClaimTransactionJournal : GetTransactionJournalViewEndpoint<ExpenseClaim>
    {
    }
}
