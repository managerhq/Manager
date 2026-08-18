using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.RealizedInvestmentGainsSummary
{
    [ProtoContract]
    internal sealed class GetRealizedInvestmentGainsSummaryView : GetReportView<Model.RealizedInvestmentGainsSummary>
    {
        protected override string DefaultTitle => Strings.RealizedInvestmentGainsLosses;

        protected override ReportModel Build(Database business, Model.RealizedInvestmentGainsSummary report)
        {
            var baseCurrency = business.Single<BaseCurrency>();

            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            model.Columns.Add(new Column { Name = Strings.Qty, HideTotals = true });
            model.Columns.Add(new Column { Name = Strings.AverageCost, HideTotals = true });
            model.Columns.Add(new Column { Name = Strings.TotalCost });
            model.Columns.Add(new Column { Name = Strings.ConsiderationReceived });
            model.Columns.Add(new Column { Name = Strings.RealizedGainsLosses, IsBold = true });

            var styles = new[] { NumberStyle.Quantity, NumberStyle.Currency, NumberStyle.Currency, NumberStyle.Currency, NumberStyle.CurrencyParentheses };
            Cell C(int i, decimal? v) => ReportNumberFormat.Cell(v, styles[i], model.WholeNumbers);

            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.GeneralLedgerAccount is BalanceSheetInvestmentsAccount || x.GeneralLedgerAccount.IsControlAccountForInvestments)
                .Where(x => x.Date <= report.ToDate)
                .GroupBy(x => x.Investment);

            foreach (var e in transactions)
            {
                var value = 0m;
                var qty = 0m;
                foreach (var e2 in e.OrderBy(x => x.Date).ThenBy(x => (x.Qty ?? 0m) < -1m))
                {
                    if (e2.Qty.HasValue && e2.Qty.Value < 0m && qty > 0m)
                    {
                        var considerationReceived = e2.BaseAmount;
                        var qtyDisposed = e2.Qty.Value * -1m;
                        var cost = baseCurrency.Round(value / qty * qtyDisposed);
                        if (qty == qtyDisposed) cost = value;
                        var gain = considerationReceived + cost;
                        if (gain != 0m)
                        {
                            if (e2.Date >= report.FromDate)
                            {
                                var allZero = qtyDisposed == 0m && cost == 0m && considerationReceived == 0m && gain == 0m;
                                if (!allZero)
                                {
                                    model.Rows.Items.Add(new Row
                                    {
                                        Name = string.Join(" - ", e2.Date.ToLocalShortDisplayString(), e.Key.GetCodeAndName()),
                                        Cells = new List<Cell>
                                        {
                                            C(0, qtyDisposed),
                                            C(1, baseCurrency.Round(cost / qtyDisposed)),
                                            C(2, cost),
                                            C(3, considerationReceived * -1m),
                                            C(4, gain * -1m),
                                        }
                                    });
                                }
                            }

                            value -= gain;
                        }
                    }

                    value += e2.BaseAmount;
                    if (e2.Qty.HasValue) qty += e2.Qty.Value;
                }
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
