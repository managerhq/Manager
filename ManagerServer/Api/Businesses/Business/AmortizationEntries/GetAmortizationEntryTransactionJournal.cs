using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.AmortizationEntries
{
    [ProtoContract]
    internal class GetAmortizationEntryTransactionJournal : GetTransactionJournalViewEndpoint<AmortizationEntry>
    {
    }
}
