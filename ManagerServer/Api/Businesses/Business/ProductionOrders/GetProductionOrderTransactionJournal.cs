using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.ProductionOrders
{
    [ProtoContract]
    internal class GetProductionOrderTransactionJournal : GetTransactionJournalViewEndpoint<ProductionOrder>
    {
    }
}
