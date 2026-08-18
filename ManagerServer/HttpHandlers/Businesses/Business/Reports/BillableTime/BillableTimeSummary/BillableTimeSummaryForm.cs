using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.BillableTimeSummary
{
    [ProtoContract]
    [Title(nameof(Strings.BillableTimeSummary), nameof(Strings.Edit))]
    [Guide("The Billable Time Summary form is used to configure report parameters.")]
    [Guide("Set date ranges and grouping options to analyze billable time by customer or project.")]
    [Fields(typeof(ManagerServer.Model.BillableTimeSummary))]
    internal sealed class BillableTimeSummaryForm : NakedVueForm<ManagerServer.Model.BillableTimeSummary>
    {
    }
}
