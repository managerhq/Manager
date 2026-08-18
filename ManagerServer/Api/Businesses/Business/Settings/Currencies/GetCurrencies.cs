using ManagerServer.Api.Businesses.Business.Settings.Currencies.BaseCurrency;
using ManagerServer.Api.Businesses.Business.Settings.Currencies.ExchangeRates;
using ManagerServer.Api.Businesses.Business.Settings.Currencies.ForeignCurrencies;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses.Business.Settings.Currencies
{
    internal sealed record CurrenciesResource(
        [property: JsonPropertyName("_links")] Dictionary<string, Link> Links);

    [ProtoContract]
    internal sealed class GetCurrencies : AuthorizedEndpoint<CurrenciesResource>
    {
        public override CurrenciesResource AuthorizedHandle()
        {
            var links = Hyperlinks.ForCurrentDocument(this);

            links["baseCurrency"] = new Link(new GetBaseCurrency { Business = Business }.ToUrl());
            links["exchangeRates"] = new Link(new GetExchangeRateBatch { Business = Business }.ToUrl());
            links["foreignCurrencies"] = new Link(new GetForeignCurrencyBatch { Business = Business }.ToUrl());

            return new CurrenciesResource(links);
        }
    }
}
