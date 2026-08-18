using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.ExpenseClaims
{
    [ProtoContract]
    [Title(nameof(Strings.ExpenseClaim))]
    [Guide("The *expense claim* view displays all the details of a submitted expense claim, including the claimant's information, line items, and total amounts.")]
    [Guide("This view provides a comprehensive summary of expenses that an employee or other authorized person has incurred on behalf of the business and is seeking reimbursement for.")]
    [Header("Available Actions")]
    [Guide("From this view, you can perform several actions:")]
    [Guide("• Click the **Edit** button to modify the expense claim details, add or remove line items, or update amounts")]
    [Guide("• Use the **Print** button to generate a printed copy of the expense claim for physical records or approval")]
    [Guide("• Process the reimbursement by creating a payment transaction directly from this view")]
    [Header("Understanding the Display")]
    [Guide("The expense claim view shows key information including the *claim date*, *reference number*, and the person who paid for the expenses.")]
    [Guide("Line items are displayed in a table format showing each expense with its description, account allocation, and amount.")]
    [Guide("If *tax codes* are applied, the tax amounts will be shown in a separate column when enabled.")]
    [LinkGuide("To learn how to create or edit expense claims, see:", typeof(ExpenseClaimForm))]
    internal sealed class ExpenseClaimView : TransactionView<ManagerServer.Model.ExpenseClaim>
    {
        protected override IEnumerable<Tuple<string, BusinessTemplate>> GetFooterButtons()
        {
            yield return new Tuple<string, BusinessTemplate>(Strings.TransactionJournal, new ExpenseClaimTransactionJournalView() { Business = Business, Key = Key, Referrer = this.ToUrl() });
        }
    }
}
