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

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.Employees
{
    [ProtoContract]
    [Title(nameof(Strings.EmployeeClearingAccount), nameof(Strings.Edit))]
    [Guide("This form configures the control account for employee clearing transactions.")]
    [Guide("The control account tracks amounts owed to or from employees.")]
    [Fields(typeof(ControlAccountForEmployees))]
    internal sealed class ControlAccountForEmployeesForm : NakedVueForm<ControlAccountForEmployees>
    {
    }
}
