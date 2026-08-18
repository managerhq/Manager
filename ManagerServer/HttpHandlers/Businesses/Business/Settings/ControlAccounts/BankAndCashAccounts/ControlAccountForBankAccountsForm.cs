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

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.BankAndCashAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.BankAndCashAccounts), nameof(Strings.Edit))]
    [Guide("This form configures the control account for bank and cash accounts.")]
    [Guide("The control account determines where bank and cash balances appear on the balance sheet.")]
    [Fields(typeof(ControlAccountForBankAccounts))]
    internal sealed class ControlAccountForBankAccountsForm : NakedVueForm<ControlAccountForBankAccounts>
    {
    }
}
