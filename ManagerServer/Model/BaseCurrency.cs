using System;
using ManagerServer.Attributes;
using ManagerServer.Model.Attributes;
using ProtoBuf;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Singleton]
    [Guid("39dde4fc-7af8-44cc-8572-3b1ff4cfb918")]
    public sealed class BaseCurrency : Currency
    {
        [Guide("Enter the three-letter ISO 4217 currency code for your base currency, such as 'USD', 'EUR', 'GBP', or your local currency code.")]
        [Guide("The base currency is your primary accounting currency - all reports and financial statements will be in this currency.")]
        [ProtoMember(3), Short, NoWrap] public string Code { get; set; }

        [Guide("Enter the full name of your base currency, such as 'US Dollar', 'Euro', or your local currency name.")]
        [Guide("This name appears in reports and helps identify your primary accounting currency throughout the system.")]
        [ProtoMember(2)] public string Name { get; set; }

        [Guide("Enter the currency symbol for your base currency, such as '$', '€', '£', or your local currency symbol which should appear as prefix.")]
        [Guide("This symbol appears with all amounts in your base currency throughout the system, making financial data easier to read.")]
        [ProtoMember(4), Short, NoWrap] public string Prefix { get; set; }

        [Guide("Enter the currency symbol for your base currency, such as '$', '€', '£', or your local currency symbol which should appear as suffix.")]
        [Guide("This symbol appears with all amounts in your base currency throughout the system, making financial data easier to read.")]
        [ProtoMember(6), Short] public string Suffix { get; set; }

        [Guide("Specify the number of decimal places for your base currency. Most currencies use 2 decimal places (e.g., $1.50).")]
        [Guide("Some currencies like Japanese Yen use 0 decimal places, while others may use 3. This setting affects how all base currency amounts are displayed and rounded.")]
        [Guide("Once set, this should not be changed as it affects all historical transactions and calculations.")]
        [ProtoMember(5), Placeholder("2")] public int? DecimalPlaces { get; set; }

        [ProtoMember(1)] public Guid? Obsolete_Currency { get; set; }

        protected override int? GetDecimalPlacesRawValue()
        {
            return DecimalPlaces;
        }

        public override string GetPrefix()
        {
            return Prefix;
        }

        public override string GetSuffix()
        {
            return Suffix;
        }

        public override string GetCode()
        {
            return Code;
        }

        public override string GetName()
        {
            return Name;
        }

        public override string GetCodeAndName()
        {
            return string.Join(" - ", Code, Name);
        }

        public decimal GetBaseAmount(decimal currencyAmount, decimal exchangeRate, bool isExchangeRateInverse, Currency currency)
        {
            if (currency is BaseCurrency) return currencyAmount;

            var exchangeRate2 = exchangeRate <= 0m ? 1m : exchangeRate;
            if (isExchangeRateInverse) return Round(currencyAmount / exchangeRate2);
            else return Round(currencyAmount * exchangeRate2);
        }

        public string GetDisplayString(ForeignCurrency foreignCurrency, decimal exchangeRate, bool exchangeRateIsInverse)
        {
            if (foreignCurrency == null) return null;
            if (exchangeRateIsInverse) return $"1 {Code} = {exchangeRate} {foreignCurrency.Code}";
            else return $"1 {foreignCurrency.Code} = {exchangeRate} {Code}";
        }
    }
}
