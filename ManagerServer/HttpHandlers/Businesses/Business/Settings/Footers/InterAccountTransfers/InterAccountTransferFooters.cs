using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.InterAccountTransfers
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(InterAccountTransfers))]
    [Title(nameof(Strings.InterAccountTransfer))]
    [Guide("Footer templates allow you to add standardized text at the bottom of *inter account transfer* forms.")]
    [Guide("Use footers to document internal money movements, add authorization notes, or include reconciliation instructions.")]
    [Header("Overview")]
    [Guide("Create multiple footer templates to handle different types of transfers between your accounts.")]
    [Guide("Each footer can contain specific text for different transfer scenarios, such as cash management transfers or foreign currency movements.")]
    [Columns]
    internal sealed class InterAccountTransferFooters : NakedObjectsWithAutomaticRows<ManagerServer.Model.InterAccountTransferFooter>
    {
        [Default]
        [Header("Creating Footer Templates")]
        [Guide("To create a new footer template, click the **New Footer** button.")]
        [Guide("Enter a descriptive name that clearly identifies the purpose of the footer, such as 'Cash Management Transfers' or 'Foreign Currency Movements'.")]
        [Header("Common Uses")]
        [Guide("Footer templates for *inter account transfers* are commonly used to include:")]
        [Guide("• Transfer authorization signatures or approval codes")]
        [Guide("• Internal control reference numbers")]
        [Guide("• Reconciliation notes or instructions")]
        [Guide("• Cash management or treasury department instructions")]
        [Guide("• Compliance or audit trail documentation")]
        [Header("Using Footer Templates")]
        [Guide("When creating an *inter account transfer*, you can select from your saved footer templates.")]
        [Guide("The selected footer text will appear at the bottom of the transfer form, providing consistent documentation across similar transfers.")]
        [Guide("This ensures proper record-keeping and helps maintain internal control procedures for money movements between accounts.")]
        public string[] GetName(ManagerServer.Model.InterAccountTransferFooter[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        protected override void OnGetNewButton()
        {
            Write(Strings.NewFooter);
        }
    }
}
