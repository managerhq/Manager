using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.LatePaymentFees
{
    [ProtoContract]
    internal class GetLatePaymentFeeTransactionJournal : GetTransactionJournalViewEndpoint<LatePaymentFee>
    {
    }
}
