using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.DivisionExceptionReport
{
    [ProtoContract]
    internal sealed class GetDivisionExceptionReportView : GetReportView<Model.DivisionExceptionReport>
    {
        protected override string DefaultTitle => Strings.DivisionExceptionReport;

        protected override ReportModel Build(Database business, Model.DivisionExceptionReport report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            var chartOfAccounts = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(Business);
            var amounts = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.BaseAmount != 0m && x.GeneralLedgerAccount.IsProfitAndLossAccount && x.Date >= report.FromDate && x.Date <= report.ToDate && x.Division == null)
                .GroupBy(x => x.GeneralLedgerAccount.Key)
                .ToDictionary(x => x.Key, x => x.Sum(y => y.BaseAmount));

            model.Columns.Add(new Column { Name = Strings.Amount });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.DebitCredit, model.WholeNumbers, link);

            foreach (var category in chartOfAccounts.ProfitAndLossStatement)
            {
                var innerRows = new Rows { HideTotals = true };
                foreach (var e2 in category.GetAllAccounts())
                {
                    if (amounts.ContainsKey(e2.Key))
                    {
                        innerRows.Items.Add(new Row
                        {
                            Name = e2.NameWithCode,
                            Cells = new System.Collections.Generic.List<Cell>
                            {
                                Make(amounts[e2.Key], new Link(new ManagerServer.HttpHandlers.Businesses.Business.Reports.DivisionExceptionReport.DivisionExceptionReportTransactions
                                {
                                    Business = Business,
                                    Referrer = Referrer,
                                    GeneralLedgerAccount = e2.Key,
                                    From = report.FromDate,
                                    To = report.ToDate,
                                }.ToUrl())),
                            }
                        });
                    }
                }
                model.Rows.Items.Add(new Row { Name = category.Name, Rows = innerRows });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
