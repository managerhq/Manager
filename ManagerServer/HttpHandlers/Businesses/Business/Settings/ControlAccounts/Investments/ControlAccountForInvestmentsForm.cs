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

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.Investments
{
    [ProtoContract]
    [Title(nameof(Strings.ControlAccount), nameof(Strings.Investments))]
    [Guide("Configure the control account for investments.")]
    [Guide("This account tracks the total value of all investment holdings.")]
    [Fields(typeof(ManagerServer.Model.ControlAccountForInvestments))]
    internal sealed class ControlAccountForInvestmentsForm : NakedVueForm<ControlAccountForInvestments>
    {
    }
}
