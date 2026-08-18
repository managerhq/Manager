using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.PayslipSummary
{
    [ProtoContract]
    [Title(nameof(Strings.PayslipSummary))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`PayslipSummary` provides a comprehensive overview of playslips, allowing you to see earnings, deductions and contributions for all employees over period of time.")]
    [Guide("To create a new `PayslipSummary`, go to `Reports` tab, click `PayslipSummary`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.PayslipSummary), name: nameof(Strings.NewReport))]
    internal sealed class PayslipSummaryList : PersistentObjectTable<ManagerServer.Model.PayslipSummary>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("5eca63c2-9b7b-4d49-8148-e18fefd17739")]
        public DateTime GetFromDate(ManagerServer.Model.PayslipSummary o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("c6e6e86c-0370-431e-b009-fd5335dda8fe")]
        public DateTime GetToDate(ManagerServer.Model.PayslipSummary o) => o.ToDate;

        [Guid("7fe05d47-bcc2-472e-8eea-fd81e1ce3407")]
        public string GetDescription(ManagerServer.Model.PayslipSummary o) => o.Description;
    }
}