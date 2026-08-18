using System.Linq;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.JournalEntries
{
    [ProtoContract]
    [Guid("500BDC05-3C3E-41FC-9AD8-5601A6C8C131")]
    [Title(nameof(Strings.JournalEntries))]
    [Guide("The **Pending Recurring Journal Entries** screen displays journal entries that are scheduled to be created automatically based on their recurring schedules.")]
    [Guide("This screen helps you manage and monitor upcoming journal entries before they are generated, allowing you to review which entries are due for processing.")]
    [Guide("Each pending entry shows when it will be created and what transactions it will contain, giving you visibility into future journal entries that will affect your accounts.")]
    [Guide("Use this screen to ensure that recurring journal entries are properly configured and will be created at the correct times according to your accounting needs.")]
    [Columns]
    internal sealed class PendingRecurringJournalEntries : NakedObjectsOfPendingRecurringTransactions<RecurringJournalEntry>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("57805628-09B7-45E3-9E40-655A9F43C009")]
        [Guide("Displays the scheduled date when each *recurring journal entry* will be automatically generated and posted to your accounts.")]
        [Guide("This date is calculated based on the recurring schedule you have configured for each journal entry, such as monthly, quarterly, or annually.")]
        public DateTime?[] GetNextIssueDate(RecurringJournalEntry[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("171B17FD-787A-43E9-A8B9-53E02E385775")]
        [Guide("Shows the *narration* text that describes the purpose or nature of each recurring journal entry.")]
        [Guide("This narration helps identify what each journal entry represents and will be included in the generated entries to maintain clear documentation of your transactions.")]
        public string[] GetNarration(RecurringJournalEntry[] rows)
        {
            return rows.Select(x => x.Narration).ToArray();
        }
    }
}