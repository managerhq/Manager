using ManagerServer.Api.Businesses.Business.Reports;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business
{
    internal abstract class GetTransactionJournalViewEndpoint<T> : Reports.GetReportView<T> where T : Model.Transaction, new()
    {
        protected override string DefaultTitle => Strings.TransactionJournal;

        protected override ReportModel Build(Database business, T transaction)
        {
            if (transaction == null) return null;

            var baseCurrency = business.Single<BaseCurrency>();
            var transactionLines = transaction.GetGeneralLedgerTransactions(business);
            var transactionCurrency = transactionLines.Where(x => !x.IsCostOfGoodsSold).FirstOrDefault()?.TransactionCurrency ?? baseCurrency;
            var foreign = transactionCurrency is not BaseCurrency;

            var model = new ReportModel
            {
                Subtitle = transaction.GetTransactionName(),
            };

            model.Columns.Add(new Column { HideTotals = true });

            if (foreign)
            {
                model.Columns.Add(new Column
                {
                    Name = Strings.ForeignCurrency,
                    Subcolumns = new List<Column>
                    {
                        new Column { Name = Strings.Debit, IsBold = true },
                        new Column { Name = Strings.Credit, IsBold = true },
                        new Column { HideTotals = true },
                    },
                });
                model.Columns.Add(new Column
                {
                    Name = Strings.BaseCurrency,
                    Subcolumns = new List<Column>
                    {
                        new Column { Name = Strings.Debit, IsBold = true },
                        new Column { Name = Strings.Credit, IsBold = true },
                    },
                });
            }
            else
            {
                model.Columns.Add(new Column { Name = Strings.Debit, IsBold = true });
                model.Columns.Add(new Column { Name = Strings.Credit, IsBold = true });
            }

            foreach (var dateGroup in transactionLines.GroupBy(x => x.Date).OrderBy(x => x.Key))
            {
                var groupItems = new List<Row>();

                foreach (var line in dateGroup)
                {
                    var cells = new List<Cell>();

                    if (line.AccountCurrency is not BaseCurrency)
                    {
                        var amount = line.AccountAmount < 0m ? decimal.Negate(line.AccountAmount) : line.AccountAmount;
                        cells.Add(new Cell { Text = amount.ToCurrencyString(line.AccountCurrency, CurrencySymbol.Short) });
                    }
                    else
                    {
                        cells.Add(new Cell());
                    }

                    if (foreign)
                    {
                        cells.Add(ReportNumberFormat.Cell(line.TransactionAmount > 0m ? line.TransactionAmount : (decimal?)null, NumberStyle.Currency, model.WholeNumbers));
                        cells.Add(ReportNumberFormat.Cell(line.TransactionAmount < 0m ? decimal.Negate(line.TransactionAmount) : (decimal?)null, NumberStyle.Currency, model.WholeNumbers));

                        if (line.ExchangeRate.HasValue)
                        {
                            cells.Add(new Cell { Text = (line.IsExchangeRateInverse ? "÷" : "×") + " " + line.ExchangeRate.ToNumberString() });
                        }
                        else
                        {
                            cells.Add(new Cell());
                        }
                    }

                    cells.Add(ReportNumberFormat.Cell(line.Debit, NumberStyle.Currency, model.WholeNumbers));
                    cells.Add(ReportNumberFormat.Cell(line.Credit, NumberStyle.Currency, model.WholeNumbers));

                    groupItems.Add(new Row { Name = line.Account, Cells = cells });
                }

                model.Rows.Items.Add(new Row
                {
                    Name = dateGroup.Key.ToLocalShortDisplayString(),
                    Rows = new Rows { Items = groupItems, HideTotals = true },
                });
            }

            model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
