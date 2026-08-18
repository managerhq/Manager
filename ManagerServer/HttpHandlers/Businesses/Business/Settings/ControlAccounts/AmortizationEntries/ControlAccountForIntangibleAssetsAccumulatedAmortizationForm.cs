using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.AmortizationEntries
{
    [ProtoContract]
    [Title(nameof(Strings.ControlAccount), nameof(Strings.IntangibleAssets), nameof(Strings.AccumulatedAmortization))]
    [Guide("Configure the control account for accumulated amortization of intangible assets.")]
    [Guide("This account tracks the total amortization recorded for all intangible assets.")]
    [Fields(typeof(ManagerServer.Model.ControlAccountForIntangibleAssetsAccumulatedAmortization))]
    internal sealed class ControlAccountForIntangibleAssetsAccumulatedAmortizationForm : NakedVueForm<ControlAccountForIntangibleAssetsAccumulatedAmortization>
    {
    }
}
