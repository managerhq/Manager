using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.PayslipTotalsPerItemAndEmployee;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.PayslipTotalsPerItemAndEmployee
{
    [ProtoContract]
    [Title(nameof(Strings.PayslipTotalsPerItemAndEmployee))]
    [Guide("The Payslip Totals Per Item and Employee report provides detailed payroll analysis.")]
    [Guide("It shows earnings, deductions, and contributions broken down by item and employee.")]
    [LinkGuide("For more information see:", typeof(PayslipTotalsPerItemAndEmployeeForm))]
    internal sealed class PayslipTotalsPerItemAndEmployeeView : DefaultView<GetPayslipTotalsPerItemAndEmployeeView>
    {
    }
}