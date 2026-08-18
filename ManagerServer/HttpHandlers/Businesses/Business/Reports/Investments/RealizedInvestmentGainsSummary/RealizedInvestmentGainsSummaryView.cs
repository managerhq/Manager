using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.RealizedInvestmentGainsSummary;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.Investments.RealizedInvestmentGainsSummary
{
    [ProtoContract]
    [Title(nameof(Strings.RealizedInvestmentGainsLosses))]
    [Guide("The **Realized Investment Gains & Losses** report shows gains and losses from investments that have been sold or disposed of during a specified period.")]
    [Guide("This report calculates the actual profit or loss realized when investments are sold by comparing the *sale proceeds* with the *average cost* of the investments.")]
    [Guide("Each disposal transaction is listed with the quantity sold, the average cost per unit, total cost basis, consideration received, and the resulting gain or loss.")]
    [Guide("Gains are shown as positive amounts, while losses are shown in parentheses. The report provides a total of all realized gains and losses for the selected period.")]
    [LinkGuide("For more information, see:", typeof(RealizedInvestmentGainsSummaryForm))]
    internal sealed class RealizedInvestmentGainsSummaryView : DefaultView<GetRealizedInvestmentGainsSummaryView>
    {
    }
}