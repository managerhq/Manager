using ManagerServer.Attributes;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("d6bc0979-e474-4188-b881-d76b1a18c964")]
    public sealed class StartingExchangeRates : Object
    {
        [Guide("List of starting exchange rates")]
        [ProtoMember(1)] public Item[] Items { get; set; }

        [ProtoContract]
        public sealed class Item
        {
            [Guide("Select foreign currency")]
            [ProtoMember(1), Autocomplete(typeof(ForeignCurrency))] public Guid? ForeignCurrency { get; set; }
            [Guide("Enter exchange rate")]
            [ProtoMember(2), IfNotNull(nameof(ForeignCurrency)), Prepend("1 {{ (lineItem.ExchangeRateIsInverse ? baseCurrency.code : lineItem.ForeignCurrency.Code) }} = "), Append("{{ (lineItem.ExchangeRateIsInverse ? lineItem.ForeignCurrency.Code : baseCurrency.code) }}")] public decimal ExchangeRate { get; set; }
            [Guide("Specify whether exchange rate is inversed")]
            [ProtoMember(3), IfNotNull(nameof(ForeignCurrency)), Icon("fa-right-left"), NoLabel] public bool ExchangeRateIsInverse { get; set; }
        }

        public Item GetExchangeRate(Currency currency)
        {
            if (currency is ForeignCurrency)
            {
                if (Items == null) return new Item() { ExchangeRate = 1m };
                foreach (var e in Items)
                {
                    if (e.ForeignCurrency == currency.Key)
                    {
                        if (e.ExchangeRate > 0m)
                        {
                            return e;
                        }
                    }
                }
            }
            return new Item() { ExchangeRate = 1m };
        }
    }
}
