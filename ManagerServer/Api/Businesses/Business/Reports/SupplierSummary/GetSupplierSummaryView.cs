using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.SupplierSummary
{
    [ProtoContract]
    internal sealed class GetSupplierSummaryView : GetReportView<Model.SupplierSummary>
    {
        protected override string DefaultTitle => Strings.SupplierSummary;

        protected override ReportModel Build(Database business, Model.SupplierSummary report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            var totals = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.GeneralLedgerAccount.IsAccountsPayable)
                .Where(x => x.Date >= report.FromDate && x.Date <= report.ToDate)
                .GroupBy(x => new { x.Supplier, TransactionType = x.Transaction.GetType() })
                .Select(x => new { x.Key.Supplier, x.Key.TransactionType, Amount = x.Sum(y => y.AccountAmount) * -1m, Currency = x.First().AccountCurrency })
                .ToList();

            totals.AddRange(new ManagerServer.Query.GeneralLedger.GeneralLedger(Business)
                .Where(x => x.GeneralLedgerAccount.IsAccountsPayable)
                .Where(x => x.Date < report.FromDate)
                .GroupBy(x => x.Supplier)
                .Select(x => new { Supplier = x.Key, TransactionType = default(Type), Amount = x.Sum(y => y.AccountAmount) * -1m, Currency = x.First().AccountCurrency }));

            var types = totals.Where(x => x.TransactionType != null).GroupBy(x => x.TransactionType).OrderByDescending(x => x.Key == typeof(ManagerServer.Model.PurchaseInvoice)).ThenByDescending(x => x.Count()).Select(x => x.Key).ToArray();

            model.Columns.Add(new Column() { Name = Strings.OpeningBalance });
            foreach (var e in types)
            {
                if (e == typeof(ManagerServer.Model.Payment)) model.Columns.Add(new Column() { Name = Strings.Payments });
                else if (e == typeof(ManagerServer.Model.Receipt)) model.Columns.Add(new Column() { Name = Strings.Refunds });
                else if (e == typeof(ManagerServer.Model.PurchaseInvoice)) model.Columns.Add(new Column() { Name = Strings.Invoices });
                else if (e == typeof(ManagerServer.Model.DebitNote)) model.Columns.Add(new Column() { Name = Strings.DebitNotes });
                else if (e == typeof(ManagerServer.Model.JournalEntry)) model.Columns.Add(new Column() { Name = Strings.JournalEntries });
                else model.Columns.Add(new Column() { Name = e.Name });
            }
            model.Columns.Add(new Column() { Name = Strings.ClosingBalance, IsBold = true });

            Cell Make(decimal? v) => ReportNumberFormat.Cell(v, NumberStyle.Currency, model.WholeNumbers);

            if (report.Division.HasValue)
            {
                totals = totals.Where(x => x.Supplier.Division == report.Division.Value).ToList();
                model.Subtitle2 = business.SingleOrDefault<ManagerServer.Model.Division>(report.Division)?.Name;
            }

            var multipleCurrencies = totals.Select(x => x.Currency).Distinct().Count() > 1;

            foreach (var e in totals.GroupBy(x => x.Currency).OrderByDescending(x => x.Key is ManagerServer.Model.BaseCurrency).ThenBy(x => x.Key.GetCode()))
            {
                var groupItems = new System.Collections.Generic.List<Row>();
                foreach (var e2 in e.GroupBy(x => x.Supplier).OrderBy(x => x.Key.Name))
                {
                    var closingBalance = 0m;
                    var openingBalance = e2.Where(x => x.TransactionType == null).Sum(x => x.Amount);
                    closingBalance += openingBalance;

                    var cells = new System.Collections.Generic.List<Cell>();
                    cells.Add(Make(openingBalance));

                    foreach (var e3 in types)
                    {
                        var amount = e2.SingleOrDefault(x => x.TransactionType == e3)?.Amount;
                        cells.Add(Make(amount));
                        if (amount.HasValue) closingBalance += amount.Value;
                    }
                    cells.Add(Make(closingBalance));

                    // ExcludeIfZero: skip row when all cells are zero
                    if (cells.All(c => (c.Value ?? 0m) == 0m)) continue;

                    var row = new Row { Name = e2.Key.NameWithCode, Cells = cells };

                    if (multipleCurrencies) groupItems.Add(row);
                    else model.Rows.Items.Add(row);
                }
                if (multipleCurrencies)
                {
                    model.Rows.Items.Add(new Row
                    {
                        Name = e.Key.GetCode(),
                        Rows = new Rows { Items = groupItems },
                    });
                }
            }

            if (!multipleCurrencies) model.Rows.Items.Add(new Row { IsTotalRow = true });

            return model;
        }
    }
}
