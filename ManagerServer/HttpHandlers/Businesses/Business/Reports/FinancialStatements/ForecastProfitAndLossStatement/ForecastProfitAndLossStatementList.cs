using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ForecastProfitAndLossStatement
{
    [ProtoContract]
    [Title(nameof(Strings.ForecastProfitAndLossStatement))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`ForecastProfitAndLossStatement` gives insights into your business's future financial health, a key tool for predicting revenue, expenses, and overall profitability.")]
    [Guide("To create a new `ForecastProfitAndLossStatement`, go to `Reports` tab, click `ForecastProfitAndLossStatement`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.ForecastProfitAndLossStatement), name: nameof(Strings.NewReport))]
    internal sealed class ForecastProfitAndLossStatementList : PersistentObjectTable<ManagerServer.Model.ForecastProfitAndLossStatement>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("d1838c03-7b4f-401f-a26d-2581ef1ae25c")]
        public DateTime? GetFromDate(ManagerServer.Model.ForecastProfitAndLossStatement o) => o.Periods?[0].FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("187fac12-4fa0-4add-98af-eee9c48ac077")]
        public DateTime? GetToDate(ManagerServer.Model.ForecastProfitAndLossStatement o) => o.Periods?[0].ToDate;

        [Guid("30eae766-971c-4164-84e8-1b92c2982fc5")]
        public string GetDescription(ManagerServer.Model.ForecastProfitAndLossStatement o) => o.Description;
    }
}