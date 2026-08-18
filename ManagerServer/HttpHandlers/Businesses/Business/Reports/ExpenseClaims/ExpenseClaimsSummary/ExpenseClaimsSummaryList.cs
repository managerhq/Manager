using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ExpenseClaimsSummary
{
    [ProtoContract]
    [Title(nameof(Strings.ExpenseClaimsSummary))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`ExpenseClaimsSummary` provides a comprehensive overview of all recorded expense claims for the period of time.")]
    [Guide("To create a new `ExpenseClaimsSummary`, go to `Reports` tab, click `ExpenseClaimsSummary`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.ExpenseClaimsSummary), name: nameof(Strings.NewReport))]
    internal sealed class ExpenseClaimsSummaryList : PersistentObjectTable<ManagerServer.Model.ExpenseClaimsSummary>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("b8dd36bd-2b9b-4f5f-85d9-7cf9dbe7c8f8")]
        public DateTime GetFromDate(ManagerServer.Model.ExpenseClaimsSummary o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("784163ca-372e-497d-8540-3a36f100e7ed")]
        public DateTime GetToDate(ManagerServer.Model.ExpenseClaimsSummary o) => o.ToDate;

        [Guid("64f4524d-2d9a-4f8b-8ff2-01ca52ed5b48")]
        public string GetDescription(ManagerServer.Model.ExpenseClaimsSummary o) => o.Description;
    }
}