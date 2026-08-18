using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.Currencies.RealizedCurrencyGainsLosses
{
    [ProtoContract]
    [Title(nameof(Strings.RealizedCurrencyGainsAndLosses), nameof(Strings.Edit))]
    [Guide("The Realized Currency Gains/Losses form configures the report parameters.")]
    [Guide("Set date ranges to analyze foreign exchange gains and losses from completed transactions.")]
    [Fields(typeof(ManagerServer.Model.RealizedCurrencyGainsLosses))]
    internal sealed class RealizedCurrencyGainsLossesForm : NakedVueForm<ManagerServer.Model.RealizedCurrencyGainsLosses>
    {
    }
}