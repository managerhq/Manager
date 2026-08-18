using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.Investments.RealizedInvestmentGainsSummary
{
    [ProtoContract]
    [Title(nameof(Strings.RealizedInvestmentGainsLosses))]
    [Guide("The *Realized Investment Gains & Losses* report calculates gains or losses from investments that have been sold or otherwise disposed of within a specific period.")]
    [Guide("This report helps you track the actual profit or loss realized when you sell investments, which is important for tax reporting and performance analysis.")]
    [Guide("To create a new report, go to the **Reports** tab, click **Realized Investment Gains & Losses**, then click the **New Report** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.RealizedInvestmentGainsLosses), name: nameof(Strings.NewReport))]
    internal sealed class RealizedInvestmentGainsSummaryList : NakedObjectsWithAutomaticRows<ManagerServer.Model.RealizedInvestmentGainsSummary>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewReport);
        }

        [Default]
        [Center, MinWidth]
        [WhitespaceNoWrap]
        public DateTime[] GetFromDate(ManagerServer.Model.RealizedInvestmentGainsSummary[] rows)
        {
            return rows.Select(x => x.FromDate).ToArray();
        }

        [Default]
        [Center, MinWidth]
        [WhitespaceNoWrap]
        public DateTime[] GetToDate(ManagerServer.Model.RealizedInvestmentGainsSummary[] rows)
        {
            return rows.Select(x => x.ToDate).ToArray();
        }

        [Default]
        public string[] GetDescription(ManagerServer.Model.RealizedInvestmentGainsSummary[] rows)
        {
            return rows.Select(x => string.Empty).ToArray();
        }
    }
}