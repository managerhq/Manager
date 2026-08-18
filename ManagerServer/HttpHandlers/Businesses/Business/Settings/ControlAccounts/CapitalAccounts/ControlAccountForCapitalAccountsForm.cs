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

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.CapitalAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.CapitalAccounts), nameof(Strings.Edit))]
    [Guide("This form configures the control account for capital accounts.")]
    [Guide("The control account determines where owner equity balances appear on the balance sheet.")]
    [Fields(typeof(ControlAccountForCapitalAccounts))]
    internal sealed class ControlAccountForCapitalAccountsForm : NakedVueForm<ControlAccountForCapitalAccounts>
    {
    }
}
