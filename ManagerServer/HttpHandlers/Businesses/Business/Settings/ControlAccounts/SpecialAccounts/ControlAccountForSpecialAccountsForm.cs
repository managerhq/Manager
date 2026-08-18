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

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.SpecialAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.ControlAccount), nameof(Strings.SpecialAccounts))]
    [Guide("Configure the control account for special accounts.")]
    [Guide("This account tracks balances for all special account sub-accounts.")]
    [Fields(typeof(ManagerServer.Model.ControlAccountForSpecialAccounts))]
    internal sealed class ControlAccountForSpecialAccountsForm : NakedVueForm<ControlAccountForSpecialAccounts>
    {
    }
}
