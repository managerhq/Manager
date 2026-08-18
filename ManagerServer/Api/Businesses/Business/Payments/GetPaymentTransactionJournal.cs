using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Payments
{
    [ProtoContract]
    internal class GetPaymentTransactionJournal : GetTransactionJournalViewEndpoint<Payment>
    {
    }
}
