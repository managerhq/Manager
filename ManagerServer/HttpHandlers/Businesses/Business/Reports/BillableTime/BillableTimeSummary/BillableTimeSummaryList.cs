using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.BillableTimeSummary
{
    [ProtoContract]
    [Title(nameof(Strings.BillableTimeSummary))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`BillableTimeSummary` provides a comprehensive overview of the time recorded for billable activities, helping you efficiently track and manage your invoicing and project costs.")]
    [Guide("To create a new `BillableTimeSummary`, go to `Reports` tab, click `BillableTimeSummary`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.BillableTimeSummary), name: nameof(Strings.NewReport))]
    internal sealed class BillableTimeSummaryList : PersistentObjectTable<ManagerServer.Model.BillableTimeSummary>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("b63874b9-d1de-4303-8d17-12b9d3744635")]
        public DateTime GetFromDate(ManagerServer.Model.BillableTimeSummary o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("5ae4ca0c-fae1-4cc8-9ea6-3a224d2e48f2")]
        public DateTime GetToDate(ManagerServer.Model.BillableTimeSummary o) => o.ToDate;

        [Guid("a30ecbb0-e68d-4544-9c7c-bc5a513f0853")]
        public string GetDescription(ManagerServer.Model.BillableTimeSummary o) => o.Description;
    }
}