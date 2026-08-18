using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.ExpenseClaimsSummary;
using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.ExpenseClaimsSummary
{
    [ProtoContract]
    internal sealed class GetExpenseClaimsSummaryView : GetReportView<Model.ExpenseClaimsSummary>
    {
        protected override string DefaultTitle => Strings.ExpenseClaimsSummary;

        protected override ReportModel Build(Database business, Model.ExpenseClaimsSummary report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            model.Columns.Add(new Column { Name = Strings.OpeningBalance });
            model.Columns.Add(new Column { Name = Strings.ExpenseClaims });
            model.Columns.Add(new Column { Name = Strings.Payments });
            model.Columns.Add(new Column { Name = Strings.Net_movement });
            model.Columns.Add(new Column { Name = Strings.ClosingBalance, IsBold = true });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.Currency, model.WholeNumbers, link);

            var expenseClaimPayers = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.GeneralLedgerAccount.Key == ManagerServer.Model.Master.AccountKeys.ExpenseClaims)
                .GroupBy(x => x.ExpenseClaimPayer)
                .OrderBy(x => x.Key.Name);

            foreach (var e in expenseClaimPayers)
            {
                var openingBalance = e.Where(x => x.Date < report.FromDate).Sum(x => x.AccountAmount) * -1m;
                var expenseClaims = e.Where(x => x.Date >= report.FromDate && x.Date <= report.ToDate).Where(x => x.Transaction is ExpenseClaim).Sum(x => x.AccountAmount) * -1m;
                var payments = e.Where(x => x.Date >= report.FromDate && x.Date <= report.ToDate).Where(x => !(x.Transaction is ExpenseClaim)).Sum(x => x.AccountAmount) * -1m;
                var netMovement = expenseClaims + payments;
                var closingBalance = openingBalance + netMovement;

                var allZero = openingBalance == 0m && expenseClaims == 0m && payments == 0m && closingBalance == 0m;
                if (allZero) continue;

                model.Rows.Items.Add(new Row
                {
                    Name = e.Key.Name,
                    Cells = new List<Cell>
                    {
                        Make(openingBalance),
                        Make(expenseClaims, new Link(new ExpenseClaimsSummaryTransactions { Business = Business, Referrer = Referrer, ExpenseClaimsPayer = e.Key.Key, From = report.FromDate, To = report.ToDate, ExpenseClaims = true }.ToUrl())),
                        Make(payments, new Link(new ExpenseClaimsSummaryTransactions { Business = Business, Referrer = Referrer, ExpenseClaimsPayer = e.Key.Key, From = report.FromDate, To = report.ToDate, Payments = true }.ToUrl())),
                        Make(netMovement, new Link(new ExpenseClaimsSummaryTransactions { Business = Business, Referrer = Referrer, ExpenseClaimsPayer = e.Key.Key, From = report.FromDate, To = report.ToDate, ExpenseClaims = true, Payments = true }.ToUrl())),
                        Make(closingBalance),
                    }
                });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
