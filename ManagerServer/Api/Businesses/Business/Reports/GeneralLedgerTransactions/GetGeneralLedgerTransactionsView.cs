using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.GeneralLedgerTransactions
{
    [ProtoContract]
    internal sealed class GetGeneralLedgerTransactionsView : GetReportModelEndpoint<Model.GeneralLedgerTransactions>
    {
        protected override string DefaultTitle => Strings.GeneralLedgerTransactions;

        protected override V2.ReportModel2 Build(Database business, Model.GeneralLedgerTransactions report)
        {
            var model = new V2.ReportModel2();
            model.Subtitles.Add(string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString()));

            model.Columns.Add(new V2.Column { Name = Strings.Debit, IsBold = true });
            model.Columns.Add(new V2.Column { Name = Strings.Credit, IsBold = true });
            model.Columns.Add(new V2.Column { Name = Strings.Balance, HideTotals = true });

            static V2.Cell Debit(decimal? v, Link link = null) => new V2.Cell { Value = v, Link = link, Style = V2.NumberStyle.Currency };
            static V2.Cell Credit(decimal? v, Link link = null) => new V2.Cell { Value = v, Link = link, Style = V2.NumberStyle.Currency };
            static V2.Cell Bal(decimal? v) => new V2.Cell { Value = v, Style = V2.NumberStyle.DebitCredit };

            var chartOfAccounts = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(Business);
            var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .DisposeFixedAssets()
                .DisposeIntangibleAssets()
                .Revaluate(report.FromDate, report.ToDate);

            foreach (var e in chartOfAccounts.ProfitAndLossStatement.SelectMany(x => x.GetAllAccounts()))
            {
                if (report.Account.HasValue && e.Key != report.Account.Value) continue;

                var groupItems = new List<V2.Row>();
                var balance = 0m;

                foreach (var e2 in transactions.Where(x => x.GeneralLedgerAccount.Key == e.Key && x.Date >= report.FromDate && x.Date <= report.ToDate && x.BaseAmount != 0m).OrderBy(x => x.Date.Date).ThenBy(x => x.Transaction?.GetName()).ToArray())
                {
                    balance += e2.BaseAmount;
                    var viewHandler = ManagerServer.HttpHandlers.Businesses.Business.TransactionViewer.GetViewHandler(Business, e2.Transaction, Referrer);
                    var link = viewHandler != null ? new Link(viewHandler.ToUrl()) : null;
                    groupItems.Add(new V2.Row
                    {
                        Name = string.Join(" — ", new[] { e2.Date.ToLocalShortDisplayString(), e2.Transaction?.GetName(), e2.Customer?.Name, e2.Supplier?.Name, e2.Employee?.Name, e2.TransactionLine?.GetDescriptionOrNull(e2.Transaction) ?? e2.Transaction?.GetDescriptionOrNull() }.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()),
                        Cells = new List<V2.Cell>
                        {
                            Debit(e2.BaseAmount > 0m ? (decimal?)e2.BaseAmount : null, link),
                            Credit(e2.BaseAmount < 0m ? (decimal?)e2.BaseAmount * -1 : null, link),
                            Bal(balance),
                        },
                    });
                }

                if (groupItems.Count == 0) continue;

                model.Rows.Add(new V2.Row
                {
                    Name = e.NameWithCode,
                    Rows = groupItems,
                });
            }

            if (!report.Account.HasValue && model.Rows.Count > 0)
            {
                var aggregated = V2.Row.Combine(model.Rows.ToArray());
                var debitTotal = aggregated.Cells != null && aggregated.Cells.Count > 0 ? aggregated.Cells[0]?.Value ?? 0m : 0m;
                var creditTotal = aggregated.Cells != null && aggregated.Cells.Count > 1 ? aggregated.Cells[1]?.Value ?? 0m : 0m;
                var net = debitTotal - creditTotal;
                model.Rows.Add(new V2.Row
                {
                    Name = Strings.ProfitLossForThePeriod,
                    IsBold = true,
                    Cells = new List<V2.Cell>
                    {                        
                        Debit(net < 0m ? (decimal?)-net : null),
                        Credit(net > 0m ? (decimal?)net : null),
                        new V2.Cell { Style = V2.NumberStyle.DebitCredit },
                    },
                });
            }

            foreach (var e in chartOfAccounts.BalanceSheet.SelectMany(x => x.GetAllAccounts()))
            {
                if (report.Account.HasValue && e.Key != report.Account.Value) continue;

                var groupItems = new List<V2.Row>();
                var balance = transactions.Where(x => x.BalanceSheetAccount.Key == e.Key && x.Date < report.FromDate).Sum(x => x.BaseAmount);

                if (balance != 0m)
                {
                    groupItems.Add(new V2.Row
                    {
                        Name = string.Join(" — ", new[] { report.FromDate.ToShortDateString(), Strings.OpeningBalance }.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()),
                        Cells = new List<V2.Cell>
                        {
                            Debit(balance > 0m ? (decimal?)balance : null),
                            Credit(balance < 0m ? (decimal?)balance * -1 : null),
                            Bal(balance),
                        },
                    });
                }

                foreach (var e2 in transactions.Where(x => x.GeneralLedgerAccount.Key == e.Key && x.Date >= report.FromDate && x.Date <= report.ToDate && x.BaseAmount != 0m).OrderBy(x => x.Date.Date).ThenBy(x => x.Transaction?.GetName()).ToArray())
                {
                    balance += e2.BaseAmount;
                    var viewHandler = ManagerServer.HttpHandlers.Businesses.Business.TransactionViewer.GetViewHandler(Business, e2.Transaction, Referrer);
                    var link = viewHandler != null ? new Link(viewHandler.ToUrl()) : null;
                    groupItems.Add(new V2.Row
                    {
                        Name = string.Join(" — ", new[] { e2.Date.ToLocalShortDisplayString(), e2.Transaction?.GetName(), e2.Customer?.NameWithCode, e2.Supplier?.NameWithCode, e2.Employee?.NameWithCode, e2.TransactionLine?.GetDescriptionOrNull(e2.Transaction) ?? e2.Transaction?.GetDescriptionOrNull() }.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()),
                        Cells = new List<V2.Cell>
                        {
                            Debit(e2.BaseAmount > 0m ? (decimal?)e2.BaseAmount : null, link),
                            Credit(e2.BaseAmount < 0m ? (decimal?)e2.BaseAmount * -1 : null, link),
                            Bal(balance),
                        },
                    });
                }

                var profit = transactions.Where(x => x.BalanceSheetAccount.Key == e.Key && x.GeneralLedgerAccount.Key != e.Key && x.Date >= report.FromDate && x.Date <= report.ToDate).Sum(x => x.BaseAmount);
                if (profit != 0m)
                {
                    balance += profit;
                    groupItems.Add(new V2.Row
                    {
                        Name = string.Join(" — ", new[] { report.ToDate.ToShortDateString(), Strings.ProfitLossForThePeriod }.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()),
                        Cells = new List<V2.Cell>
                        {
                            Debit(profit > 0m ? profit : default(decimal?)),
                            Credit(profit < 0m ? profit * -1m : default(decimal?)),
                            Bal(balance),
                        },
                    });
                }

                if (groupItems.Count == 0) continue;

                model.Rows.Add(new V2.Row
                {
                    Name = e.NameWithCode,
                    Rows = groupItems,
                });
            }

            if (!report.Account.HasValue && model.Rows.Count > 0)
            {
                var grandTotal = V2.Row.Combine(model.Rows.ToArray());
                grandTotal.Name = Strings.Total;
                grandTotal.IsBold = true;
                if (grandTotal.Cells != null && grandTotal.Cells.Count >= 3)
                {
                    grandTotal.Cells[2] = new V2.Cell { Style = V2.NumberStyle.DebitCredit };
                }
                model.Rows.Add(grandTotal);
            }

            model.Format();
            return model;
        }
    }
}
