using ManagerServer.Attributes;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.WebServices.ExchangeRates
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.ExchangeRates))]
    [Guide("Configure web service for automatic exchange rate updates.")]
    [Guide("Exchange rates will be fetched daily from the configured service.")]
    [Fields(typeof(ManagerServer.Model.WebServiceForExchangeRates))]
    internal class WebServiceForExchangeRatesForm : NakedVueForm<ManagerServer.Model.WebServiceForExchangeRates>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            var o = ApplicationData.Businesses.Get(Business).Single<WebServiceForExchangeRates>();
            if (o.Enabled) return false;
            return true;
        }
    }
}
