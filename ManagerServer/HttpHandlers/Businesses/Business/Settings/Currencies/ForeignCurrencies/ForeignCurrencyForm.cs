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

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Currencies.ForeignCurrencies
{
    [ProtoContract]
    [Title(nameof(Strings.ForeignCurrency))]
    [Guide("Define foreign currencies for multi-currency transactions.")]
    [Guide("Set currency code, symbol, and decimal places for accurate display.")]
    [Fields(typeof(ManagerServer.Model.ForeignCurrency))]
    internal sealed class ForeignCurrencyForm : NakedVueForm<ManagerServer.Model.ForeignCurrency>
    {
    }
}