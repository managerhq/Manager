using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.JournalEntries
{
    [ProtoContract]
    internal class GetJournalEntryTransactionJournal : GetTransactionJournalViewEndpoint<JournalEntry>
    {
    }
}
