using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.ExpenseClaimsSummary;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ExpenseClaimsSummary
{
    [ProtoContract]
    [Title(nameof(Strings.ExpenseClaimsSummary))]
    [Guide("The Expense Claims Summary report shows expense claim balances by payer.")]
    [Guide("It tracks movements and balances for employee reimbursements.")]
    [LinkGuide("For more information see:", typeof(ExpenseClaimsSummaryForm))]
    internal sealed class ExpenseClaimsSummaryView : DefaultView<GetExpenseClaimsSummaryView>
    {
    }
}