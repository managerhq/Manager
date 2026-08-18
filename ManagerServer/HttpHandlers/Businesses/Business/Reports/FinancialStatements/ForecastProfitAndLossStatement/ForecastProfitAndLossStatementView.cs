using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.ForecastProfitAndLossStatement;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ForecastProfitAndLossStatement
{
    [ProtoContract]
    [Title(nameof(Strings.ForecastProfitAndLossStatement))]
    [Guide("The Forecast Profit & Loss Statement projects future financial performance.")]
    [Guide("It uses forecast data to estimate income and expenses for planning purposes.")]
    [LinkGuide("For more information see:", typeof(ForecastProfitAndLossStatementForm))]
    internal sealed class ForecastProfitAndLossStatementView : DefaultView<GetForecastProfitAndLossStatementView>
    {
        protected override Tuple<string, BusinessTemplate> GetFooterAction()
        {
            return new Tuple<string, BusinessTemplate>(Strings.NewReport+" &mdash; "+Strings.ProfitAndLossStatementActualVsBudget, new ProfitAndLossStatementActualVsBudget.ProfitAndLossStatementActualVsBudgetForm() { Business = Business, Source = Key });
        }
    }
}