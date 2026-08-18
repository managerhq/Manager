using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.JournalEntries
{
    [ProtoContract]
    internal sealed class GetJournalEntry : GetObjectEndpoint<Model.JournalEntry>
    {
    }
}
