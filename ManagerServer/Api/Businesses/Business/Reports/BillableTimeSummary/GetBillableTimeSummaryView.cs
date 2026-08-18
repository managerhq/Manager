using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.BillableTimeSummary
{
    [ProtoContract]
    internal sealed class GetBillableTimeSummaryView : GetReportView<Model.BillableTimeSummary>
    {
        protected override string DefaultTitle => Strings.BillableTimeSummary;

        protected override ReportModel Build(Database business, Model.BillableTimeSummary report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            model.Columns.Add(new Column { Name = Strings.OpeningBalance });
            model.Columns.Add(new Column { Name = Strings.NewBillableTime });
            model.Columns.Add(new Column { Name = Strings.Invoiced });
            model.Columns.Add(new Column { Name = Strings.WrittenOff });
            model.Columns.Add(new Column { Name = Strings.ClosingBalance, IsBold = true });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.CurrencyParentheses, model.WholeNumbers, link);

            var currencies = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.Date <= report.ToDate && x.GeneralLedgerAccount.Key == ManagerServer.Model.Master.AccountKeys.BillableTimeUnbilled).GroupBy(x => x.AccountCurrency);

            foreach (var e in currencies)
            {
                var groupRows = new Rows();
                foreach (var e2 in e.GroupBy(x => x.Customer).OrderBy(x => x.Key.NameWithCode))
                {
                    var openingBalance = e2.Where(x => x.Date < report.FromDate).Sum(x => x.AccountAmount);
                    var billableTime = e2.Where(x => x.Date >= report.FromDate && x.AccountAmount > 0m).Sum(x => x.AccountAmount);
                    var salesInvoices = e2.Where(x => x.Date >= report.FromDate && x.AccountAmount < 0m && x.SalesInvoice != null).Sum(x => x.AccountAmount);
                    var adjustments = e2.Where(x => x.Date >= report.FromDate && x.AccountAmount < 0m && x.SalesInvoice == null).Sum(x => x.AccountAmount);
                    var closingBalance = e2.Sum(x => x.AccountAmount);

                    var allZero = openingBalance == 0m && billableTime == 0m && salesInvoices == 0m && adjustments == 0m && closingBalance == 0m;
                    if (allZero) continue;

                    groupRows.Items.Add(new Row
                    {
                        Name = e2.Key.NameWithCode,
                        Cells = new List<Cell>
                        {
                            Make(openingBalance),
                            Make(billableTime),
                            Make(salesInvoices),
                            Make(adjustments),
                            Make(closingBalance),
                        }
                    });
                }
                model.Rows.Items.Add(new Row { Name = e.Key.GetCode(), Rows = groupRows });
            }

            if (model.Rows.Items.Count == 1)
            {
                var singleGroup = model.Rows.Items[0];
                model.Rows.Items.Clear();
                foreach (var r in singleGroup.Rows.Items) model.Rows.Items.Add(r);
                model.Rows.Items.Add(new Row { IsTotalRow = true });
            }

            return model;
        }
    }
}
