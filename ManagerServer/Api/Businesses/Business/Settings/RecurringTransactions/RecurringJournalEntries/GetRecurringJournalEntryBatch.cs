namespace ManagerServer.Api.Businesses.Business.Settings.RecurringTransactions.RecurringJournalEntries
{
    [ProtoContract]
    internal sealed class GetRecurringJournalEntryBatch : GetObjectBatchEndpoint<Model.RecurringJournalEntry, GetRecurringJournalEntry, PostRecurringJournalEntry, PutRecurringJournalEntry, DeleteRecurringJournalEntry>
    {
    }
}
