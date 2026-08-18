using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using ManagerServer.Query;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.PayslipItems.PayslipEarningsItems
{
    [ProtoContract]
    [Title(nameof(Strings.PayslipEarningsItem))]
    [Guide("Define earnings items for employee payslips.")]
    [Guide("Earnings include regular wages, overtime, bonuses, and other payments to employees.")]
    [Fields(typeof(ManagerServer.Model.PayslipEarningsItem))]
    internal sealed class PayslipEarningsItemForm : NakedVueForm<PayslipEarningsItem>
    {
    }
}