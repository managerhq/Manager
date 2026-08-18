using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.CashFlowStatement;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.CashFlowStatement
{
    [ProtoContract]
    internal sealed class GetCashFlowStatementView : GetReportView<Model.CashFlowStatement>
    {
        protected override string DefaultTitle => Strings.CashFlowStatement;

        protected override ReportModel Build(Database business, Model.CashFlowStatement report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.Periods[0].FromDate.ToLocalShortDisplayString(), report.Periods[0].ToDate.ToLocalShortDisplayString());
            model.Subtitle2 = ManagerServer.Globalization.Strings.GetPropertyValue(report.Method.ToString());
            model.WholeNumbers = report.RoundDecimals;

            for (int i = 0; i < report.Periods.Length; i++)
            {
                var columnName = report.Periods[i].ToDate.ToLocalShortDisplayString();
                if (!string.IsNullOrWhiteSpace(report.Periods[i].ColumnName)) columnName = report.Periods[i].ColumnName;
                model.Columns.Add(new Column { Name = columnName, IsBold = (i == 0) });
            }

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.CurrencyParentheses, model.WholeNumbers, link);

            var dates = new List<DateTime>();
            dates.AddRange(report.Periods.Select(x => x.FromDate).Where(x => x > DateTime.MinValue).Select(x => x.AddDays(-1)));
            dates.AddRange(report.Periods.Select(x => x.ToDate));
            dates = dates.Distinct().ToList();

            var generalLedger = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .DisposeFixedAssets()
                .DisposeIntangibleAssets();

            var cashBasisAccounts = generalLedger.Revaluate(DateTime.MaxValue).Select(x => x.GeneralLedgerAccount).Distinct().ToList();

            foreach (var e in new[] { CashFlowStatementCategory.OperatingActivities, CashFlowStatementCategory.InvestingActivities, CashFlowStatementCategory.FinancingActivities })
            {
                string totalText;
                switch (e)
                {
                    case CashFlowStatementCategory.OperatingActivities: totalText = Strings.CashFlowsFromUsedInOperatingActivities; break;
                    case CashFlowStatementCategory.InvestingActivities: totalText = Strings.CashFlowsFromUsedInInvestingActivities; break;
                    case CashFlowStatementCategory.FinancingActivities: totalText = Strings.CashFlowsFromUsedInFinancingActivities; break;
                    default: totalText = Strings.Total; break;
                }

                var categoryInner = new Rows { TotalText = totalText };

                if (report.Method == CashFlowStatementMethod.IndirectMethod && e == CashFlowStatementCategory.OperatingActivities)
                {
                    var generalLedgerByPeriod = report.Periods.Select(x => generalLedger.Revaluate(x.FromDate, x.ToDate).Where(y => y.Date >= x.FromDate && y.Date <= x.ToDate).ToArray()).ToArray();

                    var netProfitCells = new List<Cell>();
                    foreach (var e2 in generalLedgerByPeriod)
                    {
                        var profit = e2.Where(x => x.ProfitAndLossAccount != null).Sum(x => x.BaseAmount) * -1m;
                        netProfitCells.Add(Make(profit));
                    }
                    categoryInner.Items.Add(new Row { Name = Strings.Net_profit_loss, Cells = netProfitCells });

                    var adjustmentItems = new List<Row>();
                    foreach (var e2 in cashBasisAccounts.Where(x => x.IsProfitAndLossAccount).GroupBy(x => business.SingleOrDefault<NamedObject>(x.GetCashFlowStatementGroup())).OrderByDescending(x => x.Key != null))
                    {
                        if (e2.Key != null)
                        {
                            var groupAccounts = new HashSet<Guid>(e2.Select(x => x.Key));
                            var cells = new List<Cell>();
                            for (int i = 0; i < report.Periods.Length; i++)
                            {
                                var nonOpOrNonCash = generalLedgerByPeriod[i]
                                    .Where(x => groupAccounts.Contains(x.GeneralLedgerAccount.Key))
                                    .Where(x => !x.IsInvoiceTransaction)
                                    .Where(x => x.GeneralLedgerAccount.CashFlowStatementCategory != CashFlowStatementCategory.OperatingActivities || !x.IsCashFlowStatementTransaction)
                                    .Sum(x => x.BaseAmount);
                                cells.Add(Make(nonOpOrNonCash, new Link(new CashFlowStatementProfitAndLossAccountGroupTransactions { Business = Business, Referrer = Referrer, CashFlowStatementGroup = e2.Key.Key, From = report.Periods[i].FromDate, To = report.Periods[i].ToDate }.ToUrl())));
                            }
                            adjustmentItems.Add(new Row { Name = e2.Key.GetName(), Cells = cells });
                        }
                        else
                        {
                            foreach (var e3 in e2.OrderBy(x => x.GetName()))
                            {
                                var inactive = false;
                                if (e3 is ProfitAndLossStatementAccount pla) inactive = pla.Inactive;
                                if (e3 is BalanceSheetAccount bsa) inactive = bsa.Inactive;

                                var cells = new List<Cell>();
                                for (int i = 0; i < report.Periods.Length; i++)
                                {
                                    var nonOpOrNonCash = generalLedgerByPeriod[i]
                                        .Where(x => x.GeneralLedgerAccount.Key == e3.Key)
                                        .Where(x => !x.IsInvoiceTransaction)
                                        .Where(x => x.GeneralLedgerAccount.CashFlowStatementCategory != CashFlowStatementCategory.OperatingActivities || !x.IsCashFlowStatementTransaction)
                                        .Sum(x => x.BaseAmount);
                                    cells.Add(Make(nonOpOrNonCash, new Link(new CashFlowStatementProfitAndLossAccountTransactions { Business = Business, Referrer = Referrer, Account = e3.Key, From = report.Periods[i].FromDate, To = report.Periods[i].ToDate }.ToUrl())));
                                }
                                if (inactive && cells.All(c => (c.Value ?? 0m) == 0m)) continue;
                                adjustmentItems.Add(new Row { Name = e3.GetName(), Cells = cells });
                            }
                        }
                    }
                    categoryInner.Items.Add(new Row
                    {
                        Name = Strings.AdjustmentsToReconcileNetProfitLossToNetCashFromOperatingActivities,
                        Rows = new Rows { Items = adjustmentItems, HideTotals = true },
                    });

                    var workingCapitalItems = new List<Row>();
                    foreach (var e2 in cashBasisAccounts.Where(x => x.CashFlowStatementCategory == e).Where(x => !x.IsProfitAndLossAccount).GroupBy(x => business.SingleOrDefault<NamedObject>(x.GetCashFlowStatementGroup())).OrderByDescending(x => x.Key != null))
                    {
                        if (e2.Key != null)
                        {
                            var groupAccounts = new HashSet<Guid>(e2.Select(x => x.Key));
                            var cells = new List<Cell>();
                            for (int i = 0; i < report.Periods.Length; i++)
                            {
                                var txns = generalLedgerByPeriod[i]
                                    .Where(x => groupAccounts.Contains(x.GeneralLedgerAccount.Key))
                                    .Where(x => x.IsCashFlowStatementTransaction)
                                    .ToArray();
                                var txns2 = generalLedgerByPeriod[i]
                                    .Where(x => groupAccounts.Contains(x.GeneralLedgerAccount.Key))
                                    .Where(x => x.IsInvoiceTransaction && x.IsBalancing)
                                    .SelectMany(x => x.ContraTransactions)
                                    .Where(x => x.GeneralLedgerAccount.IsProfitAndLossAccount)
                                    .ToArray();
                                var amount = txns.Sum(x => x.BaseAmount) * -1m + txns2.Sum(x => x.BaseAmount);
                                cells.Add(Make(amount, new Link(new CashFlowStatementWorkingCapitalGroupTransactions { Business = Business, Referrer = Referrer, CashFlowStatementGroup = e2.Key.Key, From = report.Periods[i].FromDate, To = report.Periods[i].ToDate }.ToUrl())));
                            }
                            workingCapitalItems.Add(new Row { Name = e2.Key.GetName(), Cells = cells });
                        }
                        else
                        {
                            foreach (var e3 in e2.OrderBy(x => x.GetName()))
                            {
                                if (e3.IsCashAtBank) continue;
                                var inactive = false;
                                if (e3 is ProfitAndLossStatementAccount pla) inactive = pla.Inactive;
                                if (e3 is BalanceSheetAccount bsa) inactive = bsa.Inactive;

                                var cells = new List<Cell>();
                                for (int i = 0; i < report.Periods.Length; i++)
                                {
                                    var txns = generalLedgerByPeriod[i]
                                        .Where(x => x.GeneralLedgerAccount.Key == e3.Key)
                                        .Where(x => x.IsCashFlowStatementTransaction)
                                        .ToArray();
                                    var txns2 = generalLedgerByPeriod[i]
                                        .Where(x => x.GeneralLedgerAccount.Key == e3.Key)
                                        .Where(x => x.IsInvoiceTransaction && x.IsBalancing)
                                        .SelectMany(x => x.ContraTransactions)
                                        .Where(x => x.GeneralLedgerAccount.IsProfitAndLossAccount)
                                        .ToArray();
                                    var movement = txns.Sum(x => x.BaseAmount) * -1m + txns2.Sum(x => x.BaseAmount);
                                    cells.Add(Make(movement, new Link(new CashFlowStatementWorkingCapitalAccountTransactions { Business = Business, Referrer = Referrer, Account = e3.Key, From = report.Periods[i].FromDate, To = report.Periods[i].ToDate }.ToUrl())));
                                }
                                if (inactive && cells.All(c => (c.Value ?? 0m) == 0m)) continue;
                                workingCapitalItems.Add(new Row { Name = e3.GetName(), Cells = cells });
                            }
                        }
                    }
                    categoryInner.Items.Add(new Row
                    {
                        Name = Strings.ChangesInWorkingCapital,
                        Rows = new Rows { Items = workingCapitalItems, HideTotals = true },
                    });
                }
                else
                {
                    foreach (var e2 in cashBasisAccounts.Where(x => x.CashFlowStatementCategory == e).GroupBy(x => business.SingleOrDefault<NamedObject>(x.GetCashFlowStatementGroup())).OrderByDescending(x => x.Key != null))
                    {
                        if (e2.Key != null)
                        {
                            var groupAccounts = new HashSet<Guid>(e2.Select(x => x.Key));
                            var cells = new List<Cell>();
                            for (int i2 = 0; i2 < report.Periods.Length; i2++)
                            {
                                var txns = generalLedger
                                    .Where(x => groupAccounts.Contains(x.GeneralLedgerAccount.Key))
                                    .Where(x => x.IsCashFlowStatementTransaction)
                                    .Where(x => x.Date >= report.Periods[i2].FromDate && x.Date <= report.Periods[i2].ToDate)
                                    .ToArray();
                                var amount = txns.Sum(x => x.BaseAmount) * -1m;
                                cells.Add(Make(amount, new Link(new CashFlowStatementGroupTransactions { Business = Business, Referrer = Referrer, CashFlowStatementGroup = e2.Key.Key, From = report.Periods[i2].FromDate, To = report.Periods[i2].ToDate }.ToUrl())));
                            }
                            categoryInner.Items.Add(new Row { Name = e2.Key.GetName(), Cells = cells });
                        }
                        else
                        {
                            foreach (var e3 in e2.OrderBy(x => x.GetName()))
                            {
                                if (e3.IsCashAtBank) continue;
                                var inactive = false;
                                if (e3 is ProfitAndLossStatementAccount pla) inactive = pla.Inactive;
                                if (e3 is BalanceSheetAccount bsa) inactive = bsa.Inactive;

                                var cells = new List<Cell>();
                                for (int i2 = 0; i2 < report.Periods.Length; i2++)
                                {
                                    var txns = generalLedger
                                        .Where(x => x.GeneralLedgerAccount.Key == e3.Key)
                                        .Where(x => x.IsCashFlowStatementTransaction)
                                        .Where(x => x.Date >= report.Periods[i2].FromDate && x.Date <= report.Periods[i2].ToDate)
                                        .ToArray();
                                    var amount = txns.Sum(x => x.BaseAmount) * -1m;
                                    cells.Add(Make(amount, new Link(new CashFlowStatementAccountTransactions { Business = Business, Referrer = Referrer, Account = e3.Key, From = report.Periods[i2].FromDate, To = report.Periods[i2].ToDate }.ToUrl())));
                                }
                                if (inactive && cells.All(c => (c.Value ?? 0m) == 0m)) continue;
                                categoryInner.Items.Add(new Row { Name = e3.GetName(), Cells = cells });
                            }
                        }
                    }
                }

                model.Rows.Items.Add(new Row { Name = ManagerServer.Globalization.Strings.GetPropertyValue(e.ToString()), Rows = categoryInner });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true, Name = Strings.NetIncreaseOrDecreaseInCashHeld });

            var cashAtBeginningCells = new List<Cell>();
            var foreignExchangeCells = new List<Cell>();
            var adjustmentCells = new List<Cell>();
            bool hasForeignExchange = false;
            bool hasAdjustments = false;

            for (int i = 0; i < report.Periods.Length; i++)
            {
                var generalLedger2 = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                    .Revaluate(report.Periods[i].FromDate, report.Periods[i].ToDate)
                    .GroupBy(x => x.GeneralLedgerAccount)
                    .Where(x => x.Key.CashFlowStatementCategory != CashFlowStatementCategory.OperatingActivities)
                    .Where(x => x.Key.CashFlowStatementCategory != CashFlowStatementCategory.FinancingActivities)
                    .Where(x => x.Key.CashFlowStatementCategory != CashFlowStatementCategory.InvestingActivities)
                    .SelectMany(x => x)
                    .ToArray();

                var amount = generalLedger2.Where(x => x.Date < report.Periods[i].FromDate).Sum(x => x.BaseAmount);
                var amount2 = generalLedger2.Where(x => x.Date == report.Periods[i].ToDate && x.Transaction == null).Sum(x => x.BaseAmount);
                var amount3 = generalLedger2.Where(x => x.Date >= report.Periods[i].FromDate && x.Date <= report.Periods[i].ToDate && x.JournalEntry != null && !x.JournalEntry.CashTransactionForCashFlowStatementPurposes).Sum(x => x.BaseAmount);

                cashAtBeginningCells.Add(Make(amount));
                foreignExchangeCells.Add(Make(amount2));
                adjustmentCells.Add(Make(amount3));

                if (amount2 != 0m) hasForeignExchange = true;
                if (amount3 != 0m) hasAdjustments = true;
            }

            model.Rows.Items.Add(new Row { Name = Strings.CashAtTheBeginningOfThePeriod, Cells = cashAtBeginningCells });
            if (hasForeignExchange) model.Rows.Items.Add(new Row { Name = Strings.ForeignExchangeGain, Cells = foreignExchangeCells });
            if (hasAdjustments) model.Rows.Items.Add(new Row { Name = Strings.Adjustments, Cells = adjustmentCells });
            model.Rows.Items.Add(new Row { IsTotalRow = true, Name = Strings.CashAtTheEndOfThePeriod });

            model.Footer = report.Footer;
            model.Prune(report.ExcludeZeroBalances);

            return model;
        }
    }
}
