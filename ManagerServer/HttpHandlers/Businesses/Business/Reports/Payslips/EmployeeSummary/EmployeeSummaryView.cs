using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.EmployeeSummary;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.EmployeeSummary
{
    [ProtoContract]
    [Title(nameof(Strings.EmployeeSummary))]
    [Guide("The Employee Summary report shows payroll totals for an individual employee.")]
    [Guide("It breaks down earnings, deductions, and contributions for the specified period.")]
    [LinkGuide("For more information see:", typeof(EmployeeSummaryForm))]
    internal sealed class EmployeeSummaryView : DefaultView<GetEmployeeSummaryView>
    {
    }
}