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

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.BankAndCashAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.StartingBalance), nameof(Strings.BankOrCashAccount), nameof(Strings.Edit))]
    [Guide("This form is the place where you can set up starting balance for bank or cash account.")]
    [Guide("The form includes the following fields:")]
    [Fields(typeof(BankOrCashAccountStartingBalance))]
    internal sealed class BankOrCashAccountStartingBalanceForm : NakedVueForm<BankOrCashAccountStartingBalance>
    {
        protected override bool CanHaveImage() => true;
    }
}