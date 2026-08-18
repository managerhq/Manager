using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.CapitalAccountsSummary;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CapitalAccountsSummary
{
    [ProtoContract]
    [Title(nameof(Strings.CapitalAccountsSummary))]
    [Guide("The Capital Accounts Summary report shows movements in capital accounts by subaccount.")]
    [Guide("It displays opening balances, transactions by category, and closing balances.")]
    [LinkGuide("For more information see:", typeof(CapitalAccountsSummaryForm))]
    internal sealed class CapitalAccountsSummaryView : DefaultView<GetCapitalAccountsSummaryView>
    {
    }
}