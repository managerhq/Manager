using System.Linq;
using ManagerServer.Model;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.ExpenseClaims
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("b15c32ea-a6d5-4be2-9f41-0a13c776115a")]
    [Title(nameof(Strings.ExpenseClaims))]
    [Guide("The **Expense Claims** tab tracks out-of-pocket expenses incurred by employees or members that your business will reimburse.")]
    [Guide("You can record each expense claim with details such as the amount, description, and who paid for the expense.")]
    [Guide("Once recorded, these claims can be processed for reimbursement, ensuring accurate tracking and proper financial reporting.")]
    [Header("Getting Started")]
    [Guide("To create a new expense claim, click the **New Expense Claim** button.")]
    [HeroButtonScreenshot(nameof(Strings.ExpenseClaims), nameof(Strings.NewExpenseClaim))]
    [Guide("Each expense claim entry captures essential information about business expenses paid by individuals who need reimbursement.")]
    [Header("Understanding the Columns")]
    [Guide("The **Expense Claims** tab displays the following information for each claim:")]
    [Columns]
    internal sealed class ExpenseClaims : NakedObjectsWithAutomaticRows<ExpenseClaim>
    {
        [Default]
        [Center, MinWidth]
        [WhitespaceNoWrap]
        [WarnIfFutureDate]
        [Guid("3ce6ad67-71db-4c57-b817-11334227f601")]
        [Guide("The date when the expense was incurred by the employee or member.")]
        [Guide("This date is used for proper accounting period allocation and expense tracking.")]
        public DateTime[] GetDate(ExpenseClaim[] rows)
        {
            return rows.Select(x => x.Date).ToArray();
        }

        [WarnIfNotUnique]
        [PaddedSorting]
        [Guid("9ba84ef5-16e5-4edc-be81-75ede7b6cb5d")]
        [Guide("A unique reference number for the expense claim.")]
        [Guide("This reference helps identify and track individual expense claims for processing and reimbursement.")]
        public string[] GetReference(ExpenseClaim[] rows)
        {
            return rows.Select(x => x.Reference).ToArray();
        }

        [Default]
        [Guid("07a17225-a529-4a63-9c67-27b9e8e69bc5")]
        [Guide("The person or account that paid for the expense on behalf of the business.")]
        [Guide("This can be an *Employee*, *Capital Account*, or *Expense Claims Payer*, depending on who incurred the expense.")]
        [Guide("The system will track this amount as payable to the selected payer for reimbursement.")]
        public string[] GetPaidBy(ExpenseClaim[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.Employee>(x.PaidBy)?.Name ?? database.SingleOrDefault<ManagerServer.Model.CapitalAccount>(x.PaidBy)?.Name ?? database.SingleOrDefault<ManagerServer.Model.ExpenseClaimsPayer>(x.PaidBy)?.Name).ToArray();
        }

        [Default]
        [Guid("8bf71026-d398-487b-8772-db0058571485")]
        [Guide("The name of the person or business that received the payment from the payer.")]
        [Guide("This is typically the vendor, supplier, or service provider who was paid for goods or services.")]
        public string[] GetPayee(ExpenseClaim[] rows)
        {
            return rows.Select(x => x.Payee).ToArray();
        }

        [Default]
        [Guid("2b008ffb-b96b-4b33-8569-9aca1a262659")]
        [Guide("A brief description of the expense.")]
        [Guide("Include relevant details about what was purchased or the service that was provided.")]
        public string[] GetDescription(ExpenseClaim[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }

        [Guid("236c32bc-32b2-46a5-90b4-f4edfa7e0bee")]
        [Guide("The accounts from your *Chart of Accounts* where this expense is categorized.")]
        [Guide("Multiple accounts may be shown if the expense was split across different expense categories.")]
        public string[] GetAccounts(ExpenseClaim[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => string.Join(", ", x.GetGeneralLedgerTransactions(database).Where(x => !x.IsBalancing).Select(x => x.Account).Distinct())).ToArray();
        }

        [Bold]
        [Default]
        [Sum, Right]
        [Guid("0f25f3d6-4432-4d0d-a28d-819da6af2f49")]
        [Guide("The total amount of the expense claim.")]
        [Guide("This represents the full amount that needs to be reimbursed to the payer.")]
        public Tuple<decimal, Currency>[] GetAmount(ExpenseClaim[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.GetGeneralLedgerTransactions(database).FirstOrDefault(x => x.IsBalancing)?.GetReversedTransactionAmountWithCurrency() ?? new Tuple<decimal, Currency>(0m, null)).ToArray();
        }

        protected override void OnFooterEndSection(Context context)
        {
            using (A(href: new ExpenseClaimLines() { Business = Business, Referrer = this.ToUrl() }.ToUrl(), @class: "btn btn-xs")) Write(Strings.ExpenseClaims + " - " + Strings.Lines);
            base.OnFooterEndSection(context);
        }
    }
}
