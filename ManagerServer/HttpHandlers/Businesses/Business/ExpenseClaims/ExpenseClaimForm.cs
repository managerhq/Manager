using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.ExpenseClaims
{
    [ProtoContract]
    [Title(nameof(Strings.ExpenseClaim), nameof(Strings.Edit))]
    [Guide("The `Expense Claim` form enables you to record and process reimbursement requests from employees, members, or other authorized persons who have incurred business expenses using their personal funds.")]
    [Guide("Use this form when individuals need reimbursement for out-of-pocket business expenses such as travel costs, client entertainment, office supplies, or professional development.")]
    [Header("How Expense Claims Work")]
    [Guide("When you create an expense claim, the system records a liability showing what the business owes to the claimant.")]
    [Guide("This liability remains until you process the reimbursement through a `Payment` transaction.")]
    [Guide("Each expense line can be allocated to different accounts, projects, or tracking categories for accurate cost reporting.")]
    [Header("Creating an Expense Claim")]
    [Guide("Start by selecting the person who incurred the expenses and enter the claim date.")]
    [Guide("Add line items for each expense, specifying the account, amount, and description.")]
    [Guide("Include relevant details such as merchant names, expense dates, and business purpose for each item.")]
    [Guide("Attach receipts or other supporting documents to validate the expenses.")]
    [Header("Form Fields")]
    [Guide("Complete the following fields to create an expense claim:")]
    [Fields(typeof(ManagerServer.Model.ExpenseClaim))]
    internal sealed class ExpenseClaimForm : NakedVueForm<ManagerServer.Model.ExpenseClaim>
    {
        protected override bool CanHaveImage() => true;
    }
}
