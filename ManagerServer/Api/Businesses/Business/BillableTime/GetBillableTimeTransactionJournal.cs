using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.BillableTime
{
    [ProtoContract]
    internal class GetBillableTimeTransactionJournal : GetTransactionJournalViewEndpoint<Model.BillableTime>
    {
    }
}
