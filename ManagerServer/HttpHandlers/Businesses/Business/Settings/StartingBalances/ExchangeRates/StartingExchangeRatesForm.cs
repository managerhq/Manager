using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.ExchangeRates
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.StartingBalances), nameof(Strings.ExchangeRates))]
    [Guide("This screen allows to setup starting exchange rates. This is required if business is setting up starting balances for foreign currency accounts.")]
    internal sealed class StartingExchangeRatesForm : NakedVueForm<StartingExchangeRates>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            return !ApplicationData.Businesses.Get(Business).Exists<StartingExchangeRates>();
        }
    }
}