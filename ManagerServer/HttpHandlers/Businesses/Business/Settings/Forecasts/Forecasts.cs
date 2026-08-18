using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;
using ManagerServer.Helpers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Forecasts
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.Forecasts))]
    [Guide("The **Forecasts** screen in the **Settings** tab enables you to generate forecasts based on anticipated income and expenses.")]
    [Guide("Use forecasts to project future financial performance and create budgets for comparison with actual results.")]
    [SettingsItemScreenshot("fa-chart-line", nameof(Strings.Forecasts))]
    [Header("Creating Forecast Reports")]
    [Guide("After generating your forecasts, navigate to the **Reports** tab where you will find a new report type called **Forecast Profit and Loss Statement**.")]
    [Guide("This report allows you to view your forecasted transactions for any period you specify.")]
    [Header("Using Forecasts for Budget Comparison")]
    [Guide("The figures from your forecast report can be copied into a **Profit and Loss Statement (Actual vs Budget)** report.")]
    [Guide("This enables you to compare your actual performance against your forecasted budget.")]
    internal sealed class Forecasts : PersistentObjectTable<ManagerServer.Model.Forecast>
    {
        [MinWidth, WhitespaceNoWrap, Center]
        [Guid("a3954082-8544-45da-8dcb-0ef1fe12cffa")]
        public DateTime GetDate(ManagerServer.Model.Forecast o) => o.Date;

        [Guid("1719720a-0319-4261-aa4f-62350e1364a9")]
        public string GetDescription(ManagerServer.Model.Forecast o) => o.Description;

        [Right, WhitespaceNoWrap]
        [Guid("4a770473-7454-45ce-9a37-6f92c24bf004")]
        public Tuple<decimal, string> GetAmount(ManagerServer.Model.Forecast o)
        {
            var baseCurrency = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BaseCurrency>();
            var total = o.Lines.Sum(x => x.Amount);
            return new Tuple<decimal, string>(total, total.ToCurrencyString(baseCurrency, CurrencySymbol.Short));
        }

        [MinWidth, WhitespaceNoWrap, Center]
        [Guid("65e0f171-2c82-4be8-9b8c-e6c4c9bbe599")]
        public ManagerServer.Model.Enums.Repeat GetRepeat(ManagerServer.Model.Forecast o) => o.Repeat;
    }
}
