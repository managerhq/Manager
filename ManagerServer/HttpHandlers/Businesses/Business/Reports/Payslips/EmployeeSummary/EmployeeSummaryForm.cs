using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.EmployeeSummary
{
    [ProtoContract]
    [Title(nameof(Strings.EmployeeSummary))]
    [Guide("The Employee Summary form configures parameters for employee payroll reports.")]
    [Guide("Set date ranges and employee filters to analyze payroll data.")]
    [Fields(typeof(ManagerServer.Model.EmployeeSummary))]
    internal sealed class EmployeeSummaryForm : NakedVueForm<ManagerServer.Model.EmployeeSummary>
    {
    }
}
