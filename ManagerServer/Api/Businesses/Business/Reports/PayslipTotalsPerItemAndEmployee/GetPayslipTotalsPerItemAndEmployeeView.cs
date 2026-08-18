using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.PayslipTotalsPerItemAndEmployee
{
    [ProtoContract]
    internal sealed class GetPayslipTotalsPerItemAndEmployeeView : GetReportView<Model.PayslipTotalsPerItemAndEmployee>
    {
        protected override string DefaultTitle => Strings.PayslipTotalsPerItemAndEmployee;

        protected override ReportModel Build(Database business, Model.PayslipTotalsPerItemAndEmployee report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.Periods[0].FromDate.ToLocalShortDisplayString(), report.Periods[0].ToDate.ToLocalShortDisplayString());

            for (int i = 0; i < report.Periods.Length; i++)
            {
                var name = report.Periods[i].ColumnName;
                if (string.IsNullOrWhiteSpace(name)) name = report.Periods[i].FromDate.ToLocalShortDisplayString() + " " + report.Periods[i].ToDate.ToLocalShortDisplayString();
                model.Columns.Add(new Column { Key = "Amount" + (i + 1), Name = name });
            }

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.Currency, model.WholeNumbers, link);

            var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business);
            var employeeTransactions = generalLedger.Where(x => x.Employee != null);

            foreach (var e in employeeTransactions.Where(x => x.PayslipEarningsItem != null).GroupBy(x => x.PayslipEarningsItem).OrderBy(x => x.Key.Name))
            {
                var groupItems = new List<Row>();

                foreach (var e2 in e.GroupBy(x => x.Employee))
                {
                    var cells = new List<Cell>();
                    foreach (var period in report.Periods)
                    {
                        var total = e2.Where(x => x.Date >= period.FromDate && x.Date <= period.ToDate).Sum(x => x.BaseAmount);
                        cells.Add(Make(total));
                    }
                    if (cells.All(c => c.Value == 0m)) continue;
                    groupItems.Add(new Row { Name = e2.Key.Name, Cells = cells });
                }

                model.Rows.Items.Add(new Row
                {
                    Name = e.Key.Name,
                    Rows = new Rows { Items = groupItems },
                });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true, Name = Strings.GrossPay });

            foreach (var e in employeeTransactions.Where(x => x.PayslipDeductionItem != null).GroupBy(x => x.PayslipDeductionItem).OrderBy(x => x.Key.Name))
            {
                var groupItems = new List<Row>();

                foreach (var e2 in e.GroupBy(x => x.Employee))
                {
                    var cells = new List<Cell>();
                    foreach (var period in report.Periods)
                    {
                        var total = e2.Where(x => x.Date >= period.FromDate && x.Date <= period.ToDate).Sum(x => x.BaseAmount);
                        cells.Add(Make(total));
                    }
                    if (cells.All(c => c.Value == 0m)) continue;
                    groupItems.Add(new Row { Name = e2.Key.Name, Cells = cells });
                }

                model.Rows.Items.Add(new Row
                {
                    Name = e.Key.Name,
                    Rows = new Rows { Items = groupItems, IsLess = true },
                });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true, Name = Strings.NetPay });

            foreach (var e in employeeTransactions.Where(x => x.PayslipContributionItem != null && x.GeneralLedgerAccount.IsProfitAndLossAccount).GroupBy(x => x.PayslipContributionItem).OrderBy(x => x.Key.Name))
            {
                var groupItems = new List<Row>();

                foreach (var e2 in e.GroupBy(x => x.Employee))
                {
                    var cells = new List<Cell>();
                    foreach (var period in report.Periods)
                    {
                        var total = e2.Where(x => x.Date >= period.FromDate && x.Date <= period.ToDate).Sum(x => x.BaseAmount);
                        cells.Add(Make(total));
                    }
                    if (cells.All(c => c.Value == 0m)) continue;
                    groupItems.Add(new Row { Name = e2.Key.Name, Cells = cells });
                }

                model.Rows.Items.Add(new Row
                {
                    Name = e.Key.Name,
                    Rows = new Rows { Items = groupItems },
                });
            }

            model.Rows.Items.Add(new Row
            {
                Rows = new Rows
                {
                    Items = new List<Row>(),
                    TotalText = Strings.TotalContributions,
                    MakeTotalStandOut = true,
                },
            });

            return model;
        }
    }
}
