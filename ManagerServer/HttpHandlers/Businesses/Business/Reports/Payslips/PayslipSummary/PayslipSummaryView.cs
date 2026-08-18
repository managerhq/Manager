using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Query;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.PayslipSummary;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.PayslipSummary
{
    [ProtoContract]
    [Title(nameof(Strings.PayslipSummary))]
    [Guide("The Payslip Summary report shows payroll totals by employee.")]
    [Guide("It displays gross pay, deductions, net pay, and employer contributions.")]
    [LinkGuide("For more information see:", typeof(PayslipSummaryForm))]
    internal sealed class PayslipSummaryView : DefaultView<GetPayslipSummaryView>
    {
    }
}