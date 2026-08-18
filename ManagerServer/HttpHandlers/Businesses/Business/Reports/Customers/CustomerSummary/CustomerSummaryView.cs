using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.CustomerSummary;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.CustomerSummary
{
    [ProtoContract]
    [Title(nameof(Strings.CustomerSummary))]
    [Guide("The Customer Summary report shows customer activity and balances for a specified period.")]
    [Guide("It displays opening balances, transactions by type, and closing balances for each customer.")]
    [LinkGuide("For more information see:", typeof(CustomerSummaryForm))]
    internal sealed class CustomerSummaryView : DefaultView<GetCustomerSummaryView>
    {
    }
}