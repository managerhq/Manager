using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.StatementOfChangesInEquity
{
    [ProtoContract]
    [Title(nameof(Strings.StatementOfChangesInEquity))]
    [Guide("The Statement of Changes in Equity form configures equity movement reports.")]
    [Guide("Set date ranges to analyze changes in capital and retained earnings.")]
    [Fields(typeof(ManagerServer.Model.StatementOfChangesInEquity))]
    internal sealed class StatementOfChangesInEquityForm : NakedVueForm<ManagerServer.Model.StatementOfChangesInEquity>
    {
    }
}
