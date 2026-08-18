using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CapitalAccountsSummary
{
    [ProtoContract]
    [Title(nameof(Strings.CapitalAccountsSummary), nameof(Strings.Edit))]
    [Guide("The Capital Accounts Summary form is used to configure report parameters.")]
    [Guide("Set date ranges and options to analyze capital account movements and balances.")]
    [Fields(typeof(ManagerServer.Model.CapitalAccountsSummary))]
    internal sealed class CapitalAccountsSummaryForm : NakedVueForm<ManagerServer.Model.CapitalAccountsSummary>
    {
    }
}
