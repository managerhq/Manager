using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.StatementOfChangesInEquity
{
    [ProtoContract]
    [Title(nameof(Strings.StatementOfChangesInEquity))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`StatementOfChangesInEquity` report provides a detailed overview of how the equity of your business has evolved over a specific period, reflecting all adjustments and movements in equity.")]
    [Guide("To create a new `StatementOfChangesInEquity`, go to `Reports` tab, click `StatementOfChangesInEquity`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.StatementOfChangesInEquity), name: nameof(Strings.NewReport))]
    internal sealed class StatementOfChangesInEquityList : PersistentObjectTable<ManagerServer.Model.StatementOfChangesInEquity>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("170c83ca-4841-459b-afdb-6c6691403ade")]
        public DateTime? GetFromDate(ManagerServer.Model.StatementOfChangesInEquity o) => o.Periods?[0].FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("f1e366d2-1016-4aa6-896e-8e5732968c9c")]
        public DateTime? GetToDate(ManagerServer.Model.StatementOfChangesInEquity o) => o.Periods?[0].ToDate;

        [Guid("b87be001-8ba0-4c42-b25d-bdb801d6b81b")]
        public string GetDescription(ManagerServer.Model.StatementOfChangesInEquity o) => o.Description;
    }
}