using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.InventoryWriteOffs
{
    [ProtoContract]
    internal class GetInventoryWriteOffTransactionJournal : GetTransactionJournalViewEndpoint<InventoryWriteOff>
    {
    }
}
