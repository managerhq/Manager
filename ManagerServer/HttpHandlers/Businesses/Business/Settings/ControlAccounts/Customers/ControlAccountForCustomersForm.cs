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

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.Customers
{
    [ProtoContract]
    [Title(nameof(Strings.AccountsReceivable))]
    [Guide("This form configures the control account for customer accounts receivable.")]
    [Guide("The control account determines where customer balances appear on financial statements.")]
    [Fields(typeof(ManagerServer.Model.ControlAccountForCustomers))]
    internal sealed class ControlAccountForCustomersForm : NakedVueForm<ControlAccountForCustomers>
    {
    }
}
