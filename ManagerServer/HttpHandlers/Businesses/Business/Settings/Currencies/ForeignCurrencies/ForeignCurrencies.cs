using System.Linq;
using ManagerServer.Model;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Currencies.ForeignCurrencies
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("ed8c9910-17aa-4468-a41a-f12d843accf4")]
    [Title(nameof(Strings.ForeignCurrencies))]
    [Guide("The **Foreign Currencies** screen is where you can create and manage the list of foreign currencies used in your business.")]
    [Guide("Foreign currencies allow you to record transactions in currencies other than your base currency and track exchange rate fluctuations.")]
    [Guide("To access the **Foreign Currencies** screen, go to the **Settings** tab, then click **Currencies**.")]
    [SettingsItemScreenshot(icon: "fa-coin", name: nameof(Strings.Currencies))]
    [Guide("Within the **Currencies** screen, click on **Foreign Currencies**.")]
    [Guide("To create a new foreign currency, click the **New Foreign Currency** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.ForeignCurrencies), name: nameof(Strings.NewForeignCurrency))]
    internal sealed class ForeignCurrencies : NakedObjectsWithAutomaticRows<ForeignCurrency>
    {
        [Default]
        [MinWidth, Center]
        [WarnIfNotUnique]
        [Guid("3fbe3487-1bb1-4047-9f06-21a9a0b7895d")]
        public string[] GetCode(ForeignCurrency[] foreignCurrencies)
        {
            return foreignCurrencies.Select(x => x.Code).ToArray();
        }

        [Default]
        [Guid("9b5cc5a7-c5fc-4b30-8176-165ef9ca6396")]
        public string[] GetName(ForeignCurrency[] foreignCurrencies)
        {
            return foreignCurrencies.Select(x => x.Name).ToArray();
        }

        [Default]
        [Center, MinWidth, HideColumnIfAllEmpty]
        [Guid("eb7d261d-5132-4a2f-9006-30b35fe49421")]
        public string[] GetPrefix(ForeignCurrency[] foreignCurrencies)
        {
            return foreignCurrencies.Select(x => x.Prefix).ToArray();
        }

        [Default]
        [Center, MinWidth, HideColumnIfAllEmpty]
        [Guid("ac5c9ca2-d683-494a-bc9c-e898a072f4a3")]
        public string[] GetSuffix(ForeignCurrency[] foreignCurrencies)
        {
            return foreignCurrencies.Select(x => x.Suffix).ToArray();
        }

        [Default]
        [Center, MinWidth]
        [Guid("1812fc61-80bd-49b9-819d-fb196a03431c")]
        public int?[] GetDecimalPlaces(ForeignCurrency[] foreignCurrencies)
        {
            return foreignCurrencies.Select(x => (int?)x.GetDecimalPlaces()).ToArray();
        }
    }
}