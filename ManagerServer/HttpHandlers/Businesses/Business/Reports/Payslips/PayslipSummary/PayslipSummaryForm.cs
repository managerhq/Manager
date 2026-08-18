using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.PayslipSummary
{
    [ProtoContract]
    [Title(nameof(Strings.PayslipSummary))]
    [Guide("The Payslip Summary form configures parameters for payroll summary reports.")]
    [Guide("Set date ranges to analyze employee earnings, deductions, and contributions.")]
    [Fields(typeof(ManagerServer.Model.PayslipSummary))]
    internal sealed class PayslipSummaryForm : NakedVueForm<ManagerServer.Model.PayslipSummary>
    {
    }
}