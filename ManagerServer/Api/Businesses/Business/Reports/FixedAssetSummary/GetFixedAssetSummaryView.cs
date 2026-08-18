using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.FixedAssetSummary
{
    [ProtoContract]
    internal sealed class GetFixedAssetSummaryView : GetReportView<Model.FixedAssetSummary>
    {
        protected override string DefaultTitle => Strings.FixedAssetSummary;

        protected override ReportModel Build(Database business, Model.FixedAssetSummary report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            model.Columns.Add(new Column { Name = Strings.OpeningBalance });
            model.Columns.Add(new Column { Name = Strings.AcquisitionCost });
            model.Columns.Add(new Column { Name = Strings.ConsiderationReceived });
            model.Columns.Add(new Column { Name = Strings.Depreciation });
            model.Columns.Add(new Column { Name = Strings.ProfitLoss });
            model.Columns.Add(new Column { Name = Strings.ClosingBalance, IsBold = true });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.CurrencyParentheses, model.WholeNumbers, link);

            var fixedAssets = business.OfType<ManagerServer.Model.FixedAsset>().ToDictionary(x => x.Key);
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).DisposeFixedAssets().Where(x => x.Date <= report.ToDate && x.FixedAsset != null).ToArray();
            var fixedAssetTransactions = transactions.Where(x => x.GeneralLedgerAccount.IsControlAccountForFixedAssets || x.GeneralLedgerAccount.IsControlAccountForFixedAssetsAccumulatedDepreciation).GroupBy(x => x.FixedAsset).ToDictionary(x => x.Key, x => x.ToArray());

            foreach (var e in fixedAssets.Values.OrderBy(x => x.NameWithCode))
            {
                if (!fixedAssetTransactions.ContainsKey(e)) continue;
                if (e.DisposedFixedAsset && e.DisposalDate.HasValue && e.DisposalDate.Value < report.FromDate) continue;

                var fixedAssetTransactions2 = fixedAssetTransactions[e].Where(x => x.GeneralLedgerAccount.IsControlAccountForFixedAssets).ToArray();
                var openingCost = fixedAssetTransactions2.Where(x => x.Date < report.FromDate).Sum(x => x.AccountAmount);
                var fixedAssetTransactions3 = fixedAssetTransactions[e].Where(x => x.GeneralLedgerAccount.IsControlAccountForFixedAssetsAccumulatedDepreciation).ToArray();
                var openingDepreciation = fixedAssetTransactions3.Where(x => x.Date < report.FromDate).Sum(x => x.AccountAmount);

                var additions = fixedAssetTransactions2.Where(x => !x.IsFixedAssetDisposalTransaction && x.Date >= report.FromDate && x.AccountAmount > 0m).Sum(x => x.AccountAmount);
                var disposals = fixedAssetTransactions2.Where(x => !x.IsFixedAssetDisposalTransaction && x.Date >= report.FromDate && x.AccountAmount < 0m).Sum(x => x.AccountAmount);
                var termination = fixedAssetTransactions2.Where(x => x.IsFixedAssetDisposalTransaction && x.Date >= report.FromDate).Sum(x => x.AccountAmount);

                var depreciation = fixedAssetTransactions3.Where(x => !x.IsFixedAssetDisposalTransaction && x.Date >= report.FromDate).Sum(x => x.AccountAmount);
                var depreciationTermination = fixedAssetTransactions3.Where(x => x.IsFixedAssetDisposalTransaction && x.Date >= report.FromDate).Sum(x => x.AccountAmount);

                var closingCost = fixedAssetTransactions2.Sum(x => x.AccountAmount);
                var closingDepreciation = fixedAssetTransactions3.Sum(x => x.AccountAmount);

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
