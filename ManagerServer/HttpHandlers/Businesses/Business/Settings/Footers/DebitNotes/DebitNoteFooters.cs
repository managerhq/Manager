using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.DebitNotes
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(DebitNotes))]
    [Title(nameof(Strings.DebitNote))]
    [Guide("Footer templates allow you to add standard text at the bottom of your debit notes to suppliers.")]
    [Guide("Create different footer templates for various situations where you need to charge suppliers or adjust their account balances.")]
    [Columns]
    internal sealed class DebitNoteFooters : NakedObjectsWithAutomaticRows<ManagerServer.Model.DebitNoteFooter>
    {
        [Default]
        [Guide("Footer templates are text blocks that appear at the bottom of debit notes, providing important information about additional charges or adjustments to your supplier's account.")]
        [Header("Common Uses")]
        [Guide("Use footers to explain the reason for the debit, such as *late delivery penalties*, *quality claim adjustments*, *damaged goods charges*, or *administrative fees*.")]
        [Guide("Include references to your original purchase orders, dispute procedures, or payment adjustment terms to ensure clear communication with suppliers.")]
        [Header("Creating Footer Templates")]
        [Guide("Give each footer template a descriptive name that clearly identifies its purpose. For example: 'Late Delivery Charges', 'Quality Claim Adjustments', or 'Restocking Fee'.")]
        [Guide("When creating a debit note, you can select the appropriate footer template from a dropdown list. This saves time and ensures consistency in your communications.")]
        public string[] GetName(ManagerServer.Model.DebitNoteFooter[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        protected override void OnGetNewButton()
        {
            Write(Strings.NewFooter);
        }
    }
}
