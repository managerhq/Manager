using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.IntangibleAssetSummary
{
    [ProtoContract]
    internal sealed class GetIntangibleAssetSummaryView : GetReportView<Model.IntangibleAssetSummary>
    {
        protected override string DefaultTitle => Strings.IntangibleAssetSummary;

        protected override ReportModel Build(Database business, Model.IntangibleAssetSummary report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            model.Columns.Add(new Column { Name = Strings.OpeningBalance });
            model.Columns.Add(new Column { Name = Strings.AcquisitionCost });
            model.Columns.Add(new Column { Name = Strings.ConsiderationReceived });
            model.Columns.Add(new Column { Name = Strings.Amortization });
            model.Columns.Add(new Column { Name = Strings.ProfitLoss });
            model.Columns.Add(new Column { Name = Strings.ClosingBalance, IsBold = true });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.CurrencyParentheses, model.WholeNumbers, link);

            var intangibleAssets = business.OfType<ManagerServer.Model.IntangibleAsset>().ToDictionary(x => x.Key);
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).DisposeIntangibleAssets().Where(x => x.Date <= report.ToDate && x.IntangibleAsset != null).ToArray();
            var intangibleAssetTransactions = transactions.Where(x => x.GeneralLedgerAccount.IsControlAccountForIntangibleAssets || x.GeneralLedgerAccount.IsControlAccountForIntangibleAssetsAccumulatedAmortization).GroupBy(x => x.IntangibleAsset).ToDictionary(x => x.Key, x => x.ToArray());

            foreach (var e in intangibleAssets.Values.OrderBy(x => x.ItemName))
            {
                if (!intangibleAssetTransactions.ContainsKey(e)) continue;
                if (e.DisposedIntangibleAsset && e.DisposalDate.HasValue && e.DisposalDate.Value < report.FromDate) continue;

                var intangibleAssetTransactions2 = intangibleAssetTransactions[e].Where(x => x.GeneralLedgerAccount.IsControlAccountForIntangibleAssets).ToArray();
                var openingCost = intangibleAssetTransactions2.Where(x => x.Date < report.FromDate).Sum(x => x.AccountAmount);
                var intangibleAssetTransactions3 = intangibleAssetTransactions[e].Where(x => x.GeneralLedgerAccount.IsControlAccountForIntangibleAssetsAccumulatedAmortization).ToArray();
                var openingDepreciation = intangibleAssetTransactions3.Where(x => x.Date < report.FromDate).Sum(x => x.AccountAmount);

                var additions = intangibleAssetTransactions2.Where(x => !x.IsIntangibleAssetDisposalTransaction && x.Date >= report.FromDate && x.AccountAmount > 0m).Sum(x => x.AccountAmount);
                var disposals = intangibleAssetTransactions2.Where(x => !x.IsIntangibleAssetDisposalTransaction && x.Date >= report.FromDate && x.AccountAmount < 0m).Sum(x => x.AccountAmount);
                var termination = intangibleAssetTransactions2.Where(x => x.IsIntangibleAssetDisposalTransaction && x.Date >= report.FromDate).Sum(x => x.AccountAmount);

                var depreciation = intangibleAssetTransactions3.Where(x => !x.IsIntangibleAssetDisposalTransaction && x.Date >= report.FromDate).Sum(x => x.AccountAmount);
                var depreciationTermination = intangibleAssetTransactions3.Where(x => x.IsIntangibleAssetDisposalTransaction && x.Date >= report.FromDate).Sum(x => x.AccountAmount);

                var closingCost = intangibleAssetTransactions2.Sum(x => x.AccountAmount);
                var closingDepreciation = intangibleAssetTransactions3.Sum(x => x.AccountAmount);

                // ExcludeIfZero = true in legacy for row1 and row2; build sub-rows and skip if empty
                Row row1 = null;
                if (!(openingCost == 0m && additions == 0m && disposals == 0m && termination == 0m && closingCost == 0m))
                {
                    row1 = new Row
                    {
                        Name = Strings.AtCost,
                        Cells =
                        [
                            Make(openingCost),
                            Make(additions),
                            Make(disposals),
                            new Cell(),
                            Make(termination),
                            Make(closingCost),
                        ],
                    };
                }

                Row row2 = null;
                if (!(openingDepreciation == 0m && depreciation == 0m && depreciationTermination == 0m && closingDepreciation == 0m))
                {
                    row2 = new Row
                    {
                        Name = Strings.AccumulatedDepreciation,
                        Cells =
                        [
                            Make(openingDepreciation),
                            new Cell(),
                            new Cell(),
                            Make(depreciation),
                            Make(depreciationTermination),
                            Make(closingDepreciation),
                        ],
                    };
                }

                if (row1 == null && row2 == null) continue;

                var groupItems = new System.Collections.Generic.List<Row>();
                if (row1 != null) groupItems.Add(row1);
                if (row2 != null) groupItems.Add(row2);

                model.Rows.Items.Add(new Row
                {
                    Name = e.NameWithCode,
                    Rows = new Rows { Items = groupItems },
                });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
