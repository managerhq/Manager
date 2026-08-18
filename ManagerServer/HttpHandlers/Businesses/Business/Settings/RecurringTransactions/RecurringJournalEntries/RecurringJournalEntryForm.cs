using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.RecurringTransactions.RecurringJournalEntries
{
    [ProtoContract]
    [Title(nameof(Strings.RecurringJournalEntry))]
    [Guide("Create journal entries that repeat on a regular schedule.")]
    [Guide("Useful for monthly accruals, depreciation, or other regular adjustments.")]
    [Fields(typeof(ManagerServer.Model.RecurringJournalEntry))]
    internal sealed class RecurringJournalEntryForm : NakedVueForm<ManagerServer.Model.RecurringJournalEntry>
    {
        protected override void OnSource(RecurringJournalEntry form, ManagerServer.Model.Object source)
        {
            if (source is JournalEntry journalEntry)
            {
                Copy(journalEntry, form);
            }
        }
    }
}