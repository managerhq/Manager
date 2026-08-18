using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.JournalEntries
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(JournalEntries))]
    [Title(nameof(Strings.JournalEntry))]
    [Guide("Journal entry footers are customizable text sections that appear at the bottom of your journal entries.")]
    [Guide("Use footers to add important documentation, explanations, and authorization details to your accounting adjustments.")]
    [Columns]
    internal sealed class JournalEntryFooters : NakedObjectsWithAutomaticRows<ManagerServer.Model.JournalEntryFooter>
    {
        [Default]
        [Guide("Click **New Footer** to create a template that can be reused across multiple journal entries.")]
        [Guide("Enter a descriptive name for each footer template to easily identify its purpose, such as 'Month-End Adjustments' or 'Audit Adjustment Entries'.")]
        [Header("Common Uses for Journal Entry Footers")]
        [Guide("Use footers to document *adjustment explanations* for period-end entries, corrections, or reclassifications.")]
        [Guide("Include *supporting document references* such as spreadsheet names, approval emails, or source documentation.")]
        [Guide("Add *authorization details* including approver names, dates, and internal control reference numbers.")]
        [Guide("Record *audit trail information* to maintain proper documentation for internal and external reviews.")]
        [Guide("Include *accounting standards compliance notes* when entries relate to specific regulations or policies.")]
        [Header("How to Use Footer Templates")]
        [Guide("Create multiple footer templates for different types of journal entries in your organization.")]
        [Guide("When creating a journal entry, select the appropriate footer template from the dropdown menu.")]
        [Guide("The selected footer text will automatically appear at the bottom of the journal entry.")]
        [Guide("This ensures consistent documentation across similar transactions while saving time on data entry.")]
        public string[] GetName(ManagerServer.Model.JournalEntryFooter[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        protected override void OnGetNewButton()
        {
            Write(Strings.NewFooter);
        }
    }
}
