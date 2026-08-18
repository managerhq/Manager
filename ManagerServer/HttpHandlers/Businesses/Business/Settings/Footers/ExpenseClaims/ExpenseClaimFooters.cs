using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.ExpenseClaims
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(ExpenseClaims))]
    [Title(nameof(Strings.ExpenseClaim))]
    [Guide("Footer templates allow you to add standardized text at the bottom of expense claims to communicate important information to employees.")]
    [Guide("You can create multiple footer templates tailored to different expense types, departments, or employee categories.")]
    [Columns]
    internal sealed class ExpenseClaimFooters : NakedObjectsWithAutomaticRows<ManagerServer.Model.ExpenseClaimFooter>
    {
        [Default]
        [Guide("Footers appear at the bottom of expense claims and typically contain important policy information that employees need to understand when submitting expenses.")]
        [Header("Common Uses for Footers")]
        [Guide("Footer templates are commonly used to display *expense policy reminders*, *receipt requirements*, *approval workflows*, *reimbursement timelines*, and *compliance statements*.")]
        [Guide("Different footer templates can be created for various expense categories such as travel expenses, entertainment expenses, or general office supplies.")]
        [Header("Creating Footer Templates")]
        [Guide("When creating a footer template, give it a descriptive name that clearly identifies its purpose, such as 'Travel Expense Policy' or 'Local Expense Guidelines'.")]
        [Guide("Each expense claim can use a different footer template, allowing you to provide relevant information based on the type of expenses being claimed.")]
        public string[] GetName(ManagerServer.Model.ExpenseClaimFooter[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        protected override void OnGetNewButton()
        {
            Write(Strings.NewFooter);
        }
    }
}
