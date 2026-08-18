using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using static ManagerServer.Model.Attributes.ExpressionAttribute.Operators;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("32b97957-5cc5-48f0-b104-89ae340695d4")]
    public sealed class InvestmentMarketPrice : NamedObject, IForeignCurrencyTransaction, IComparable<InvestmentMarketPrice>
    {
        [Guide("Select the date for which you are recording the market price. This is typically the date shown on a valuation statement or market quote.")]
        [ProtoMember(1)] public DateTime Date { get; set; }
        [Guide("Select the investment for which you are recording the market price.")]
        [ProtoMember(2), Autocomplete(typeof(ManagerServer.Model.Investment))] public Guid? Investment { get; set; }
        [Guide("If the investment is priced in a foreign currency, select that currency here. Leave blank if priced in your base currency.")]
        [ProtoMember(3), Autocomplete(typeof(ManagerServer.Model.ForeignCurrency))] public Guid? Currency { get; set; }
        [Guide("Enter the market price per unit/share of the investment as of the specified date.")]
        [ProtoMember(6), NoWrap, IfNotNull(nameof(Investment)), NoPlaceholder, AppendCurrency] public decimal MarketPrice { get; set; }
        [Guide("Enter the exchange rate to convert the foreign currency price to your base currency. Click Autofill to use the system exchange rate for this date.")]
        [ProtoMember(7), Placeholder(nameof(Strings.Autofill)), NoWrap, IfNotNull(nameof(Currency)), Prepend("1 {{ (ExchangeRateIsInverse ? baseCurrency.code : Currency.Code) }} = "), Append("{{ (ExchangeRateIsInverse ? Currency.Code : baseCurrency.code) }}")] public decimal ExchangeRate { get; set; }
        [Guide("Check this box if you want to enter the exchange rate in reverse format (e.g., instead of 1 USD = 0.75 EUR, enter 1 EUR = 1.33 USD).")]
        [ProtoMember(8), IfNotNull(nameof(Currency)), NoWrap, Icon("fa-right-left")] public bool ExchangeRateIsInverse { get; set; }
        [IfNotNull(nameof(Investment)), IfNotNull(nameof(Currency)), IfFalse(nameof(ExchangeRateIsInverse)), EmptyLabel, NoWrap, AppendBaseCurrency, Expression(Zero, Plus, nameof(MarketPrice), Times, nameof(ExchangeRate), RoundToBaseCurrency)] public object MarketPriceInBaseCurrency1 { get; set; }
        [IfNotNull(nameof(Investment)), IfNotNull(nameof(Currency)), IfTrue(nameof(ExchangeRateIsInverse)), EmptyLabel, AppendBaseCurrency, Expression(Zero, Plus, nameof(MarketPrice), Divide, nameof(ExchangeRate), RoundToBaseCurrency)] public object MarketPriceInBaseCurrency2 { get; set; }

        DateTime IForeignCurrencyTransaction.Date => Date;
        Guid? IForeignCurrencyTransaction.Currency => Currency;
        decimal IForeignCurrencyTransaction.ExchangeRate { get => ExchangeRate; set => ExchangeRate = value; }
        bool IForeignCurrencyTransaction.ExchangeRateIsInverse { get => ExchangeRateIsInverse; set => ExchangeRateIsInverse = value; }

        public override string GetName() => Strings.InvestmentMarketPrice + " — " + Date.ToShortDateString();

        int IComparable<InvestmentMarketPrice>.CompareTo(InvestmentMarketPrice other)
        {
            return (Date).CompareTo((other.Date));
        }

        public decimal? GetMarketPriceInBaseCurrency(BaseCurrency baseCurrency)
        {
            if (MarketPrice <= 0m) return 0m;
            if (!Currency.HasValue) return MarketPrice;
            if (ExchangeRate <= 0m) return 0m;
            if (!ExchangeRateIsInverse)
            {
                return baseCurrency.Round(MarketPrice * ExchangeRate);
            }
            else
            {
                return baseCurrency.Round(MarketPrice / ExchangeRate);
            }
        }
    }
}