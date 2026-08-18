using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.CreditNotes
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(CreditNotes))]
    [Title(nameof(Strings.Footers), nameof(Strings.CreditNotes))]
    [Guide("Credit note footers are customizable text sections that appear at the bottom of your credit notes.")]
    [Guide("Use footers to communicate important information about the credit, such as refund procedures, terms and conditions, or references to the original transaction.")]
    [Header("Managing Footer Templates")]
    [Guide("You can create multiple footer templates to handle different credit scenarios.")]
    [Guide("Each template can be tailored for specific situations like product returns, billing adjustments, or discount credits.")]
    [Columns]
    internal sealed class CreditNoteFooters : NakedObjectsWithAutomaticRows<ManagerServer.Model.CreditNoteFooter>
    {
        [Default]
        [Guide("The *Name* field identifies each footer template in your system.")]
        [Guide("Choose descriptive names that clearly indicate the template's purpose, making it easy to select the right footer when creating credit notes.")]
        [Header("Best Practices for Footer Names")]
        [Guide("Use specific names that describe the credit scenario, such as:")]
        [Guide("• 'Product Return Policy' for credits related to returned merchandise")]
        [Guide("• 'Billing Adjustment Terms' for corrections to previous invoices")]
        [Guide("• 'Volume Discount Credits' for quantity-based refunds")]
        [Guide("• 'Service Cancellation Refund' for terminated service agreements")]
        [Header("Typical Footer Content")]
        [Guide("Footer templates commonly include:")]
        [Guide("• Refund procedures and timelines")]
        [Guide("• How the credit will be applied (against future purchases or as a refund)")]
        [Guide("• Validity periods for the credit")]
        [Guide("• References to original invoice numbers")]
        [Guide("• Contact information for credit-related inquiries")]
        public string[] GetName(ManagerServer.Model.CreditNoteFooter[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        protected override void OnGetNewButton()
        {
            Write(Strings.NewFooter);
        }
    }
}