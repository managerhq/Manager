using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.DepreciationEntries
{
    [ProtoContract]
    internal class GetDepreciationEntryTransactionJournal : GetTransactionJournalViewEndpoint<DepreciationEntry>
    {
    }
}
