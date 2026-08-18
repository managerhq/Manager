using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.PayslipTotalsPerItemAndEmployee
{
    [ProtoContract]
    [Title(nameof(Strings.PayslipTotalsPerItemAndEmployee))]
    [Guide("The Payslip Totals Per Item and Employee form configures detailed payroll reports.")]
    [Guide("Set parameters to analyze payroll items broken down by employee and category.")]
    [Fields(typeof(ManagerServer.Model.PayslipTotalsPerItemAndEmployee))]
    internal sealed class PayslipTotalsPerItemAndEmployeeForm : NakedVueForm<ManagerServer.Model.PayslipTotalsPerItemAndEmployee>
    {
    }
}
