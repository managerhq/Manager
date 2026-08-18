using ManagerServer.Model;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.JournalEntries
{
    [ProtoContract]
    internal sealed class GetJournalEntryBatch : GetObjectBatchEndpoint<Model.JournalEntry, GetJournalEntry, PostJournalEntry, PutJournalEntry, DeleteJournalEntry>
    {
    }
}
