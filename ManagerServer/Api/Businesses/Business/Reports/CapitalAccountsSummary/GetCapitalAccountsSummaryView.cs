using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.CapitalAccountsSummary;
using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.CapitalAccountsSummary
{
    [ProtoContract]
    internal sealed class GetCapitalAccountsSummaryView : GetReportView<Model.CapitalAccountsSummary>
    {
        protected override string DefaultTitle => Strings.CapitalAccountsSummary;

        protected override ReportModel Build(Database business, Model.CapitalAccountsSummary report)
        {
            var model = new ReportModel();

            model.Title = report.Title;
            if (report.Rounding == ManagerServer.Model.Enums.Rounding.On) model.WholeNumbers = true;
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());
            model.Footer = report.Footer;

            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.Date <= report.ToDate && x.GeneralLedgerAccount.IsControlAccountForCapitalAccounts).ToArray();
            var capitalSubaccounts = transactions.Where(x => x.Date >= report.FromDate).GroupBy(x => x.CapitalSubaccount).OrderByDescending(x => x.Count()).Select(x => x.Key).ToArray();

            model.Columns.Add(new Column { Name = Strings.OpeningBalance });
            foreach (var e in capitalSubaccounts)
            {
                if (e == null) model.Columns.Add(new Column { Name = Strings.Suspense });
                else model.Columns.Add(new Column { Name = e.Name });
            }
            model.Columns.Add(new Column { Name = Strings.ClosingBalance, IsBold = true });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.CurrencyParentheses, model.WholeNumbers, link);

            foreach (var e in transactions.GroupBy(x => x.CapitalAccount).OrderBy(x => x.Key.Name))
            {
                var closingBalance = 0m;
                var openingBalance = e.Where(x => x.Date < report.FromDate).Sum(x => x.AccountAmount);

                if (report.ReverseSigns) openingBalance *= -1m;

                closingBalance += openingBalance;

                var cells = new List<Cell>();
                cells.Add(Make(openingBalance));
                foreach (var e2 in capitalSubaccounts)
                {
                    var movement = e.Where(x => x.Date >= report.FromDate && x.CapitalSubaccount?.Key == e2?.Key).Sum(x => x.AccountAmount);

                    if (report.ReverseSigns) movement *= -1m;

                    closingBalance += movement;
                    cells.Add(Make(movement, new Link(new CapitalAccountsSummaryTransactions { Business = Business, Referrer = Referrer, From = report.FromDate, To = report.ToDate, CapitalAccount = e.Key.Key, CapitalSubaccount = e2?.Key }.ToUrl())));
                }
                cells.Add(Make(closingBalance));

                model.Rows.Items.Add(new Row { Name = e.Key.Name, Cells = cells });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            model.Prune(report.ExcludeZeroBalances);

            return model;
        }
    }
}
