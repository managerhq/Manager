using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("14240c19-3d08-4fe6-94bb-6dd17c4bcda6")]
    public sealed class ExchangeRate : NamedObject, IComparable<ExchangeRate>
    {
        [Guide("Enter the date for this exchange rate. The system uses the rate effective on or before the transaction date when calculating foreign currency conversions.")]
        [Guide("Exchange rates should be updated regularly to ensure accurate financial reporting, especially if your business has significant foreign currency transactions.")]
        [Guide("Consider entering rates at month-end for financial reporting, or more frequently for volatile currencies.")]
        [ProtoMember(1)] public DateTime Date { get; set; }

        [Guide("Select the foreign currency this exchange rate applies to. Each foreign currency requires its own exchange rate entries.")]
        [Guide("The rate defines how many units of the selected currency equal one unit of your base currency (or vice versa if using inverse rates).")]
        [ProtoMember(2), DoNotHide, Autocomplete(typeof(ManagerServer.Model.ForeignCurrency))] public Guid? Currency { get; set; }

        [Guide("Enter the exchange rate between your base currency and the selected foreign currency.")]
        [Guide("You can enter the rate in either direction - use the inverse button to switch between formats.")]
        [Guide("For example, if 1 USD = 0.85 EUR, you can enter either 0.85 or use inverse to enter 1.18 (1 EUR = 1.18 USD).")]
        [Guide("The system automatically converts between formats, so use whichever is more convenient or matches your source data.")]
        [ProtoMember(6), IfNotNull(nameof(Currency)), NoPlaceholder, NoWrap, WebService(typeof(WebServiceForExchangeRates)), Label(nameof(Strings.ExchangeRate)), Prepend("1 {{ (ExchangeRateIsInverse ? baseCurrency.code : Currency.Code) }} = "), Append("{{ (ExchangeRateIsInverse ? Currency.Code : baseCurrency.code) }}")] public decimal ExchangeRateValue { get; set; }
        [ProtoMember(7), IfNotNull(nameof(Currency)), Icon("fa-right-left")] public bool ExchangeRateIsInverse { get; set; }

        [ProtoMember(5)] public ExchangeRateType Obsolete_Type { get; set; }
        [ProtoMember(3)] public decimal Obsolete_BaseRate { get; set; }
        [ProtoMember(4)] public decimal Obsolete_CounterRate { get; set; }

        public decimal GetExchangeRateValue() => ExchangeRateValue <= 0m ? 1m : ExchangeRateValue;
        public decimal GetBaseRate() => ExchangeRateIsInverse ? GetExchangeRateValue() : GetBaseRateFromCounterRate(GetExchangeRateValue());

        private decimal GetBaseRateFromCounterRate(decimal counterRate)
        {
            if (counterRate == 0m) return 0m;
            return 1m / counterRate;
        }

        public override string GetName() => Strings.ExchangeRate + " — " + Date.ToShortDateString();

        int IComparable<ExchangeRate>.CompareTo(ExchangeRate other)
        {
            return (Currency, Date).CompareTo((other.Currency, other.Date));
        }
    }
}