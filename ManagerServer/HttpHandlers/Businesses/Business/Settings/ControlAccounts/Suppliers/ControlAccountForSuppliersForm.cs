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

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.Suppliers
{
    [ProtoContract]
    [Title(nameof(Strings.AccountsPayable), nameof(Strings.Edit))]
    [Guide("This form configures the control account for supplier accounts payable.")]
    [Guide("The control account determines where supplier balances appear on financial statements.")]
    [Fields(typeof(ControlAccountForSuppliers))]
    internal sealed class ControlAccountForSuppliersForm : NakedVueForm<ControlAccountForSuppliers>
    {
    }
}
