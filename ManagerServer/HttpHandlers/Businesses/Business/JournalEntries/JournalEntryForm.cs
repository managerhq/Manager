using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.JournalEntries
{
    [ProtoContract]
    [Title(nameof(Strings.JournalEntry), nameof(Strings.Edit))]
    [Guide("The `JournalEntry` form enables you to create manual accounting entries for transactions that cannot be recorded through standard forms like invoices, receipts, or payments.")]
    [Guide("Journal entries provide direct access to your general ledger, allowing you to record complex transactions, adjustments, corrections, and period-end accruals.")]
    [Header("Purpose and Uses")]
    [Guide("Each journal entry must balance (debits equal credits) to maintain the integrity of your double-entry bookkeeping system.")]
    [Guide("Common uses include recording depreciation, accruals, prepayments, inter-company transactions, and year-end adjustments.")]
    [Header("Creating Journal Entries")]
    [Guide("When creating a journal entry, provide a clear description explaining the purpose of the entry for audit trail purposes.")]
    [Guide("Enter debit amounts in the debit column and credit amounts in the credit column for each affected account.")]
    [Guide("You can allocate entries to specific tracking categories like customers, suppliers, or inventory items.")]
    [Guide("Always ensure your entry balances before saving—the system will prevent unbalanced entries from being recorded.")]
    [Header("Form Fields")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.JournalEntry))]
    internal sealed class JournalEntryForm : NakedVueForm<ManagerServer.Model.JournalEntry>
    {
        protected override bool CanHaveImage() => true;

        protected override void OnSource(JournalEntry form, ManagerServer.Model.Object source)
        {
            if (source is JournalEntry journalEntry)
            {
                Copy(journalEntry, form);
            }
        }
    }
}