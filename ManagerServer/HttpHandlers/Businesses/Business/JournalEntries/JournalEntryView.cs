using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.JournalEntries
{
    [ProtoContract]
    [Title(nameof(Strings.JournalEntry))]
    [Guide("The *Journal Entry* view displays detailed information about a journal entry transaction, including its date, reference number, narration, and individual debit and credit line items.")]
    [Guide("Journal entries are the fundamental building blocks of your accounting system, recording financial transactions by debiting and crediting appropriate accounts according to double-entry bookkeeping principles.")]
    [Header("Reviewing Your Journal Entry")]
    [Guide("From this view, you can review the complete transaction details to ensure accuracy. The system automatically verifies that your debits and credits are balanced.")]
    [Guide("If your debits and credits don't match, an *Unbalanced* warning will appear at the bottom of the entry. This indicates that the entry needs to be corrected before it can properly affect your accounts.")]
    [Header("Making Changes")]
    [Guide("To make changes to this journal entry, click the **Edit** button. This will open the journal entry form where you can modify any aspect of the transaction.")]
    [Guide("You can also use the **Copy to** function to create a new journal entry or recurring journal entry based on this one. This is useful when you need to create similar transactions without starting from scratch.")]
    [LinkGuide("For more information about creating and editing journal entries, see:", typeof(JournalEntryForm))]
    internal sealed class JournalEntryView : TransactionView<ManagerServer.Model.JournalEntry>
    {
        protected override Type[] GetCopyToOptions()
        {
            return [typeof(JournalEntry), typeof(RecurringJournalEntry)];
        }

        protected override IEnumerable<Tuple<string, BusinessTemplate>> GetFooterButtons()
        {
            yield return new Tuple<string, BusinessTemplate>(Strings.TransactionJournal, new JournalEntryTransactionJournalView() { Business = Business, Key = Key, Referrer = this.ToUrl() });
        }
    }
}