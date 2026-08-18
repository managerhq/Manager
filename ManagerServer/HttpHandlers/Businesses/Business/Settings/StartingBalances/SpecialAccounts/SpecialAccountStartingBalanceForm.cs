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

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.SpecialAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.StartingBalance), nameof(Strings.SpecialAccount), nameof(Strings.Edit))]
    [Guide("This form is the place where you can set up starting balance for special account.")]
    [Guide("The form includes the following fields:")]
    [Fields(typeof(SpecialAccountStartingBalance))]
    internal sealed class SpecialAccountStartingBalanceForm : NakedVueForm<SpecialAccountStartingBalance>
    {
        protected override bool CanHaveImage() => true;
    }
}