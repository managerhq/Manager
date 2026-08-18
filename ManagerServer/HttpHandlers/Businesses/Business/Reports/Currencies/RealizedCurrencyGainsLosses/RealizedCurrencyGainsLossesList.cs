using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.Currencies.RealizedCurrencyGainsLosses
{
    [ProtoContract]
    [Title(nameof(Strings.RealizedCurrencyGainsAndLosses))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("The *Realized Currency Gains and Losses* report provides a detailed overview of the gains and losses that have been realized when foreign currency transactions are converted to your base currency.")]
    [Guide("This report helps you track the financial impact of currency fluctuations on completed transactions involving foreign currencies.")]
    [Guide("To create a new report, go to the **Reports** tab, click **Realized Currency Gains and Losses**, then click the **New Report** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.RealizedCurrencyGainsAndLosses), name: nameof(Strings.NewReport))]
    internal sealed class RealizedCurrencyGainsLossesList : PersistentObjectTable<ManagerServer.Model.RealizedCurrencyGainsLosses>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("7350615e-d5c4-4e7b-a586-4432a4d7ca1c")]
        public DateTime GetFromDate(ManagerServer.Model.RealizedCurrencyGainsLosses o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("369345c8-3abe-4375-81c1-8a84f49ca114")]
        public DateTime GetToDate(ManagerServer.Model.RealizedCurrencyGainsLosses o) => o.ToDate;

        [Guid("cae01cad-d108-45c3-bf61-d4d07af1213f")]
        public string GetDescription(ManagerServer.Model.RealizedCurrencyGainsLosses o) => o.Description;
    }
}