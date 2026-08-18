using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ExpenseClaimPayers
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(ExpenseClaims))]
    [Title(nameof(Strings.ExpenseClaimPayers))]
    [NewButton(nameof(Strings.NewExpenseClaimPayer))]
    [Guide("*Expense claim payers* are individuals or entities who pay for business expenses using their own funds and need to be reimbursed.")]
    [Guide("Use this feature to maintain a list of people who can submit expense claims to your business, such as employees, contractors, or business owners who pay for business expenses personally.")]
    [Guide("To add a new expense claim payer, click the **New Expense Claim Payer** button.")]
    [SettingsItemScreenshot("fa-user-tie", nameof(Strings.ExpenseClaimPayers))]
    internal sealed class ExpenseClaimPayers : PersistentObjectTable<ManagerServer.Model.ExpenseClaimsPayer>
    {
        [Guid("3715c870-43a8-4e04-b804-a83b5ad2cc41")]
        public string GetName(ManagerServer.Model.ExpenseClaimsPayer o) => o.Name;
    }
}
