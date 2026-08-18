using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Currencies.ExchangeRates
{
    [ProtoContract]
    [Title(nameof(Strings.ExchangeRate))]
    [Guide("Set exchange rates between foreign currencies and the base currency.")]
    [Guide("Exchange rates determine conversion values for multi-currency transactions.")]
    [Fields(typeof(ManagerServer.Model.ExchangeRate))]
    internal sealed class ExchangeRateForm : NakedVueForm<ManagerServer.Model.ExchangeRate>
    {
        [ProtoMember(1)] public Guid? ForeignCurrency;
        [ProtoMember(2)] public DateTime? Date;
        [ProtoMember(3)] public bool? ExchangeRateIsInverse;
        [ProtoMember(4)] public decimal? ExchangeRateValue;

        protected override void OnSource(ExchangeRate form, ManagerServer.Model.Object source)
        {
            if (!Key.HasValue)
            {
                if (ForeignCurrency.HasValue) form.Currency = ForeignCurrency.Value;
                if (Date.HasValue) form.Date = Date.Value;
                if (ExchangeRateIsInverse.HasValue) form.ExchangeRateIsInverse = ExchangeRateIsInverse.Value;
                if (ExchangeRateValue.HasValue) form.ExchangeRateValue = ExchangeRateValue.Value;
            }
        }
    }
}