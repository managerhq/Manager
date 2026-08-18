using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.DepreciationCalculationWorksheet;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.DepreciationCalculationWorksheet
{
    [ProtoContract]
    internal sealed class GetDepreciationCalculationWorksheetView : GetReportView<Model.DepreciationCalculationWorksheet>
    {
        protected override string DefaultTitle => Strings.DepreciationCalculationWorksheet;

        protected override ReportModel Build(Database business, Model.DepreciationCalculationWorksheet report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            model.Columns.Add(new Column { Name = Strings.RecalculatedDepreciation });
            model.Columns.Add(new Column { Name = Strings.DepreciationEntries });
            model.Columns.Add(new Column { Name = Strings.Difference, IsBold = true });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.Currency, model.WholeNumbers, link);

            var fixedAssets = business.OfType<ManagerServer.Model.FixedAsset>().ToDictionary(x => x.Key);

            foreach (var e in GetItems(Business, report))
            {
                // ExcludeIfZero = true in legacy; skip when all cells zero
                if (e.RecalculatedDepreciation == 0m && e.DepreciationEntries == 0m && e.Difference == 0m) continue;

                model.Rows.Items.Add(new Row
                {
                    Name = fixedAssets[e.FixedAsset].NameWithCode,
                    Cells =
                    [
                        Make(e.RecalculatedDepreciation),
                        Make(e.DepreciationEntries, new Link(new DepreciationCalculationWorksheetDepreciationEntries { Business = Business, Referrer = Referrer, FixedAsset = e.FixedAsset, FromDate = report.FromDate, ToDate = report.ToDate }.ToUrl())),
                        Make(e.Difference),
                    ],
                });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            // TODO: ViewModel lacks CustomButton/CustomMessage — NewDepreciationEntry button not surfaced

            return model;
        }

        public static Item[] GetItems(string fileId, ManagerServer.Model.DepreciationCalculationWorksheet report)
        {
            var list = new List<Item>();

            var database = ApplicationData.Instance.Businesses.Get(fileId);
            var baseCurrency = database.Single<ManagerServer.Model.BaseCurrency>();
            var fixedAssets = database.OfType<ManagerServer.Model.FixedAsset>().OrderBy(x => x.NameWithCode);
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(fileId).Where(x => x.Date <= report.ToDate && x.FixedAsset != null).ToArray();

            foreach (var e in fixedAssets)
            {
                if (e.DisposedFixedAsset && e.DisposalDate.HasValue && e.DisposalDate.Value < report.FromDate) continue;

                var fixedAssetTransactions = transactions.Where(x => x.FixedAsset.Key == e.Key).ToArray();

                var depreciationEntries = fixedAssetTransactions
                    .Where(x => x.GeneralLedgerAccount.IsControlAccountForFixedAssetsAccumulatedDepreciation)
                    .Where(x => x.Date >= report.FromDate && x.Date <= report.ToDate)
                    .Sum(x => x.BaseAmount) * -1m;

                var bookValue = fixedAssetTransactions
                    .Where(x => x.GeneralLedgerAccount.IsControlAccountForFixedAssets || x.GeneralLedgerAccount.IsControlAccountForFixedAssetsAccumulatedDepreciation)
                    .Where(x => x.Date < report.FromDate)
                    .Sum(x => x.BaseAmount);

                var recalculatedDepreciation = 0m;
                var lastDate = report.FromDate;
                foreach (var e2 in fixedAssetTransactions.Where(x => x.GeneralLedgerAccount.IsControlAccountForFixedAssets && x.Date >= report.FromDate).GroupBy(x => x.Date).OrderBy(x => x.Key))
                {
                    var amount = e2.Sum(x => x.BaseAmount);

                    if (bookValue > 0m)
                    {
                        var depreciationDays = (int)(e2.Key - lastDate).TotalDays;
                        var depreciation = baseCurrency.Round(bookValue / 100m * e.DepreciationRate * (depreciationDays / 365m));
                        recalculatedDepreciation += depreciation;
                    }

                    lastDate = e2.Key;
                    bookValue += amount;
                    bookValue -= recalculatedDepreciation;
                }

                if (bookValue > 0m)
                {
                    var depreciationDays = (int)(report.ToDate.AddDays(1) - lastDate).TotalDays;
                    var depreciation = baseCurrency.Round(bookValue / 100m * e.DepreciationRate * (depreciationDays / 365m));
                    recalculatedDepreciation += depreciation;
                }

                list.Add(new Item()
                {
                    FixedAsset = e.Key,
                    RecalculatedDepreciation = recalculatedDepreciation,
                    DepreciationEntries = depreciationEntries,
                    Difference = recalculatedDepreciation - depreciationEntries,
                });
            }

            return list.ToArray();
        }

        public sealed class Item
        {
            public Guid FixedAsset;
            public decimal RecalculatedDepreciation;
            public decimal DepreciationEntries;
            public decimal Difference;
        }
    }
}
