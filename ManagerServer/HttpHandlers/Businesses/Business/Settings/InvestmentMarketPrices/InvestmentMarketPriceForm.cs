using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Query;
using HttpFramework;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.InvestmentMarketPrices
{
    [ProtoContract]
    [Title(nameof(Strings.InvestmentMarketPrice))]
    [Guide("Record market prices for investments to track their current value.")]
    [Guide("Market prices are used to calculate unrealized gains and portfolio valuations.")]
    [Fields(typeof(ManagerServer.Model.InvestmentMarketPrice))]
    internal sealed class InvestmentMarketPriceForm : NakedVueForm<ManagerServer.Model.InvestmentMarketPrice>
    {
        [ProtoMember(1)] public Guid? Investment;
        [ProtoMember(2)] public DateTime? Date;
        [ProtoMember(3)] public decimal? MarketPrice;
        [ProtoMember(4)] public Guid? Currency;
        [ProtoMember(5)] public decimal? ExchangeRate;
        [ProtoMember(6)] public bool? ExchangeRateIsInverse;

        protected override void OnSource(InvestmentMarketPrice form, ManagerServer.Model.Object source)
        {
            if (!Key.HasValue)
            {
                if (Investment.HasValue) form.Investment = Investment.Value;
                if (Date.HasValue) form.Date = Date.Value;
                if (MarketPrice.HasValue) form.MarketPrice = MarketPrice.Value;
                if (Currency.HasValue) form.Currency = Currency.Value;
                if (ExchangeRate.HasValue) form.ExchangeRate = ExchangeRate.Value;
                if (ExchangeRateIsInverse.HasValue) form.ExchangeRateIsInverse = ExchangeRateIsInverse.Value;
            }
        }
    }
}