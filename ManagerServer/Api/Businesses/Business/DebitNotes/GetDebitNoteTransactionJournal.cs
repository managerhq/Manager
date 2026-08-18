using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.DebitNotes
{
    [ProtoContract]
    internal class GetDebitNoteTransactionJournal : GetTransactionJournalViewEndpoint<DebitNote>
    {
    }
}
