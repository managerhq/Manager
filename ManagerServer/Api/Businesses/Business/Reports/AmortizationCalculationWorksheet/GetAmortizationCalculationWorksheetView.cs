using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.AmortizationCalculationWorksheet;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.AmortizationCalculationWorksheet
{
    [ProtoContract]
    internal sealed class GetAmortizationCalculationWorksheetView : GetReportView<Model.AmortizationCalculationWorksheet>
    {
        protected override string DefaultTitle => Strings.AmortizationCalculationWorksheet;

        protected override ReportModel Build(Database business, Model.AmortizationCalculationWorksheet report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            model.Columns.Add(new Column { Name = Strings.BookValue });
            model.Columns.Add(new Column { Name = Strings.AmortizationRate, HideTotals = true });
            model.Columns.Add(new Column { Name = Strings.AmortizationDays, HideTotals = true });
            model.Columns.Add(new Column { Name = Strings.Amortization, IsBold = true });

            var styles = new[] { NumberStyle.Currency, NumberStyle.Percentage, NumberStyle.Quantity, NumberStyle.Currency };
            Cell MakeAt(int i, decimal? v) => ReportNumberFormat.Cell(v, styles[i], model.WholeNumbers);

            var intangibleAssets = business.OfType<ManagerServer.Model.IntangibleAsset>().ToDictionary(x => x.Key);

            foreach (var e in GetItems(Business, report))
            {
                // ExcludeIfZero = true in legacy; skip when all cells zero
                if (e.BookValue == 0m && e.AmortizationRate == 0m && e.AmortizationDays == 0 && e.Amortization == 0m) continue;

                model.Rows.Items.Add(new Row
                {
                    Name = intangibleAssets[e.IntangibleAsset].NameWithCode,
                    Cells =
                    [
                        MakeAt(0, e.BookValue),
                        MakeAt(1, e.AmortizationRate),
                        MakeAt(2, e.AmortizationDays),
                        MakeAt(3, e.Amortization),
                    ],
                });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }

        public static Item[] GetItems(string fileId, ManagerServer.Model.AmortizationCalculationWorksheet report)
        {
            var list = new List<Item>();

            var numberDecimalDigits = ApplicationData.Instance.Businesses.Get(fileId).Single<ManagerServer.Model.BaseCurrency>().GetDecimalPlaces();

            var intangibleAssets = ApplicationData.Instance.Businesses.Get(fileId).OfType<ManagerServer.Model.IntangibleAsset>().OrderBy(x => x.NameWithCode);

            var daysWithinPeriod = 0;
            if (report.ToDate >= report.FromDate)
            {
                daysWithinPeriod = (int)Math.Floor((report.ToDate - report.FromDate).TotalDays) + 1;
            }

            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(fileId).Where(x => x.Date <= report.ToDate && (x.GeneralLedgerAccount.IsControlAccountForIntangibleAssets || x.GeneralLedgerAccount.IsControlAccountForIntangibleAssetsAccumulatedAmortization)).ToArray();

            foreach (var e in intangibleAssets)
            {
                if (e.DisposedIntangibleAsset && e.DisposalDate.HasValue && e.DisposalDate.Value < report.FromDate) continue;

                var bookValue = transactions.Where(x => x.GeneralLedgerAccount.IsControlAccountForIntangibleAssets && x.IntangibleAsset.Key == e.Key).Sum(x => x.AccountAmount) + transactions.Where(x => x.GeneralLedgerAccount.IsControlAccountForIntangibleAssetsAccumulatedAmortization && x.IntangibleAsset.Key == e.Key && x.Date < report.FromDate).Sum(x => x.AccountAmount);

                var amortizationDays = daysWithinPeriod;
                if (e.DisposedIntangibleAsset && e.DisposalDate.HasValue && e.DisposalDate.Value < report.ToDate)
                {
                    amortizationDays -= (int)Math.Floor((report.ToDate - e.DisposalDate.Value).TotalDays);
                }

                var amortization = Math.Round(bookValue / 100m * e.AmortizationRate * (amortizationDays / 365m), numberDecimalDigits, MidpointRounding.AwayFromZero);

                if (amortization < 0m) amortization = 0m;

                list.Add(new Item()
                {
                    IntangibleAsset = e.Key,
                    BookValue = bookValue,
                    AmortizationRate = e.AmortizationRate,
                    AmortizationDays = amortizationDays,
                    Amortization = amortization
                });
            }

            return list.ToArray();
        }

        public sealed class Item
        {
            public Guid IntangibleAsset;
            public decimal BookValue;
            public decimal AmortizationRate;
            public int AmortizationDays;
            public decimal Amortization;
        }
    }
}
