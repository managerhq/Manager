using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.Investments.RealizedInvestmentGainsSummary
{
    [ProtoContract]
    [Title(nameof(Strings.RealizedCurrencyGainsAndLosses), nameof(Strings.Edit))]
    [Guide("The Realized Investment Gains Summary form configures parameters for gains analysis.")]
    [Guide("Set date ranges to view realized gains and losses from investment disposals.")]
    [Fields(typeof(ManagerServer.Model.RealizedInvestmentGainsSummary))]
    internal sealed class RealizedInvestmentGainsSummaryForm : NakedVueForm<ManagerServer.Model.RealizedInvestmentGainsSummary>
    {
    }
}
