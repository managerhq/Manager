using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.Receipts
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Receipts))]
    [Title(nameof(Strings.Receipt))]
    [Guide("Receipt footers are customizable text sections that appear at the bottom of your receipts.")]
    [Guide("They provide a professional way to acknowledge payments received from customers, suppliers, or other sources.")]
    [Header("Managing Receipt Footers")]
    [Guide("You can create multiple footer templates to use with different types of receipts.")]
    [Guide("Each footer template can be selected when creating or editing individual receipts.")]
    [Columns]
    internal sealed class ReceiptFooters : NakedObjectsWithAutomaticRows<ManagerServer.Model.ReceiptFooter>
    {
        [Default]
        [Guide("This list displays all your receipt footer templates.")]
        [Guide("To create a new footer template, click the **New Footer** button.")]
        [Header("What to Include in Receipt Footers")]
        [Guide("Receipt footers typically contain acknowledgment text that confirms payment has been received.")]
        [Guide("Common content includes payment confirmation messages, deposit details, thank you notes, or references to specific invoices being paid.")]
        [Guide("Different footer templates can be created for various receipt scenarios, such as customer payments, deposits, or miscellaneous income.")]
        [Header("Naming Your Footer Templates")]
        [Guide("Give each footer template a descriptive name that clearly identifies its purpose.")]
        [Guide("Examples of good footer names include 'Customer Payment Receipt', 'Deposit Acknowledgment', 'Invoice Payment Confirmation', or 'General Receipt Footer'.")]
        [Guide("Clear naming helps you quickly select the appropriate footer when creating receipts.")]
        public string[] GetName(ManagerServer.Model.ReceiptFooter[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        protected override void OnGetNewButton()
        {
            Write(Strings.NewFooter);
        }
    }
}
