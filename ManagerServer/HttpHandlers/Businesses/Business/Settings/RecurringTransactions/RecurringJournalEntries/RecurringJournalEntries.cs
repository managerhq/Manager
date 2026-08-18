using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringJournalEntries
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(JournalEntries))]
    [Guid("356f4d68-3aab-4387-9449-5fd5ba217d0f")]
    [Title(nameof(Strings.RecurringJournalEntries), nameof(Strings.Pending))]
    [Guide("Recurring journal entries allow you to automate frequently repeated journal entries in your accounting system.")]
    [Guide("Instead of manually creating the same journal entries every month, quarter, or year, you can set them up once to generate automatically according to your specified schedule.")]
    [Guide("Common uses include recording monthly depreciation, allocating recurring expenses across departments, or recognizing deferred revenue on a regular basis.")]
    [Guide("Each recurring journal entry can be configured with its own schedule, amounts, and account allocations. The system will automatically create the journal entries based on your settings.")]
    [Columns]
    internal sealed class RecurringJournalEntries : NakedObjectsWithAutomaticRows<ManagerServer.Model.RecurringJournalEntry>
    {
        [Default]
        [MinWidth, Center]
        [WhitespaceNoWrap]
        [Guid("72e897fb-69be-4076-bb75-914bd09905bf")]
        [Guide("Displays the scheduled date for the next automatic creation of each recurring journal entry.")]
        [Guide("This date is calculated based on your recurring schedule settings and updates automatically after each journal entry is generated.")]
        public DateTime?[] GetNextIssueDate(ManagerServer.Model.RecurringJournalEntry[] rows)
        {
            return rows.Select(x => x.NextIssueDate).ToArray();
        }

        [Default]
        [Guid("0ec33fb3-06bc-456d-8280-f9dd4c911d40")]
        [Guide("Shows the narration text that will be included with each generated journal entry.")]
        [Guide("This description helps identify the purpose of the journal entry and is copied to each automatically created transaction.")]
        public string[] GetNarration(ManagerServer.Model.RecurringJournalEntry[] rows)
        {
            return rows.Select(x => x.Narration).ToArray();
        }
    }
}