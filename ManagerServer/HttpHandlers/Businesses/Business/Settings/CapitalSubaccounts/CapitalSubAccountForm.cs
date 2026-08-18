using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CapitalSubaccounts
{
    [ProtoContract]
    [Title(nameof(Strings.CapitalSubaccounts), nameof(Strings.Edit))]
    [Guide("The Capital Subaccount form is used to create or edit capital subaccount categories.")]
    [Guide("Subaccounts help organize capital accounts into detailed categories.")]
    [Fields(typeof(SubAccount))]
    internal sealed class CapitalSubaccountForm : NakedVueForm<SubAccount>
    {
    }
}
