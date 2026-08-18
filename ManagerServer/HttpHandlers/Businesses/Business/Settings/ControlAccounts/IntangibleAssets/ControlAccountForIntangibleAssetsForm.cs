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

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.IntangibleAssets
{
    [ProtoContract]
    [Title(nameof(Strings.ControlAccount), nameof(Strings.IntangibleAssets))]
    [Guide("Configure the control account for intangible assets at cost.")]
    [Guide("This account tracks the purchase value of all intangible assets.")]
    [Fields(typeof(ManagerServer.Model.ControlAccountForIntangibleAssets))]
    internal sealed class ControlAccountForIntangibleAssetsForm : NakedVueForm<ControlAccountForIntangibleAssets>
    {
    }
}
