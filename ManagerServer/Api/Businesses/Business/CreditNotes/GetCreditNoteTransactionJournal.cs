using ManagerServer.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.CreditNotes
{
    [ProtoContract]
    internal class GetCreditNoteTransactionJournal : GetTransactionJournalViewEndpoint<CreditNote>
    {
    }
}
