using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("6116531b-cb3d-4f85-b239-745972943a6b")]
    public sealed class ForeignCurrency : Currency, IComparable<ForeignCurrency>, ICode
    {
        [Guide("Enter the three-letter ISO 4217 currency code, such as 'USD' for US Dollar, 'EUR' for Euro, 'GBP' for British Pound, or 'JPY' for Japanese Yen.")]
        [Guide("Using standard currency codes ensures consistency and helps with exchange rate lookups and international transactions.")]
        [Guide("The code appears throughout the system to identify this currency and is used when importing exchange rates.")]
        [ProtoMember(2), Short, NoWrap] public string Code { get; set; }

        [Guide("Enter the full name of the currency, such as 'US Dollar', 'Euro', 'British Pound', or 'Japanese Yen'.")]
        [Guide("The name helps users identify the currency in dropdown menus and reports, especially when multiple currencies are in use.")]
        [Guide("Use the official currency name for clarity and professional appearance on documents.")]
        [ProtoMember(1)] public string Name { get; set; }

        [Guide("Enter the currency symbol that will appear with amounts as prefix, such as '$' for dollars, '€' for euros, '£' for pounds, or '¥' for yen.")]
        [Guide("The symbol is displayed before or after amounts depending on your locale settings and makes financial data easier to read at a glance.")]
        [ProtoMember(3), Short, NoWrap] public string Prefix { get; set; }

        [Guide("Enter the currency symbol that will appear with amounts as suffix, such as '$' for dollars, '€' for euros, '£' for pounds, or '¥' for yen.")]
        [Guide("The symbol is displayed before or after amounts depending on your locale settings and makes financial data easier to read at a glance.")]
        [ProtoMember(9), Short] public string Suffix { get; set; }

        [Guide("Specify the number of decimal places for this currency. Most currencies use 2 decimal places (e.g., $1.50), but some use different amounts.")]
        [Guide("For example, Japanese Yen typically uses 0 decimal places (¥1,000), while some Middle Eastern currencies use 3 decimal places.")]
        [Guide("This setting ensures amounts are displayed and rounded correctly throughout the system.")]
        [ProtoMember(4), Placeholder("2")] public int? DecimalPlaces { get; set; }
        [Guide("Mark this foreign currency as inactive to hide it from dropdown lists while preserving all historical transactions.")]
        [Guide("Use this when you no longer transact in this currency but need to keep records of past transactions for reporting purposes.")]
        [Guide("Inactive currencies can be reactivated at any time if you resume trading in that currency.")]
        [ProtoMember(6)] public bool Inactive { get; set; }

        [ProtoMember(5)] public decimal Obsolete_StartingExchangeRate { get; set; }
        [ProtoMember(7)] public decimal Obsolete_DefaultExchangeRate { get; set; }
        [ProtoMember(8)] public bool Obsolete_DefaultExchangeRateIsInverse { get; set; }

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
            return Name ?? string.Empty;
        }

        public override string GetCodeAndName()
        {
            return string.Join(" - ", Code, Name);
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            return true;
        }

        public override decimal GetBaseAmount(decimal currencyAmount, decimal exchangeRate, bool isExchangeRateInverse, BaseCurrency baseCurrency)
        {
            var exchangeRate2 = exchangeRate <= 0m ? 1m : exchangeRate;
            if (isExchangeRateInverse) return baseCurrency.Round(currencyAmount / exchangeRate2);
            else return baseCurrency.Round(currencyAmount * exchangeRate2);
        }

        int IComparable<ForeignCurrency>.CompareTo(ForeignCurrency other) => other.GetName().CompareTo(this.GetName());
        string ICode.Code => Code;
    }
}