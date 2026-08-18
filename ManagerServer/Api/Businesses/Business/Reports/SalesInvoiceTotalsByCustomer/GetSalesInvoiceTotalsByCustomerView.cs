using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByCustomer;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.SalesInvoiceTotalsByCustomer
{
    [ProtoContract]
    internal sealed class GetSalesInvoiceTotalsByCustomerView : GetReportView<Model.SalesInvoiceTotalsByCustomer>
    {
        protected override string DefaultTitle => Strings.SalesInvoiceTotalsByCustomer;

        protected override ReportModel Build(Database business, Model.SalesInvoiceTotalsByCustomer report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.Periods[0].FromDate.ToLocalShortDisplayString(), report.Periods[0].ToDate.ToLocalShortDisplayString());

            for (int i = 0; i < report.Periods.Length; i++)
            {
                var columnName = report.Periods[i].ToDate.ToLocalShortDisplayString();
                if (!string.IsNullOrWhiteSpace(report.Periods[i].ColumnName)) columnName = report.Periods[i].ColumnName;
                model.Columns.Add(new Column { Name = columnName, IsBold = (i == 0) });
            }

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.CurrencyParentheses, model.WholeNumbers, link);

            var currencies = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsAccountsReceivable && (x.Transaction is SalesInvoice || x.Transaction is CreditNote)).GroupBy(x => x.AccountCurrency).OrderByDescending(x => x.Key is BaseCurrency).ThenBy(x => x.Key.GetCode());

            foreach (var e in currencies)
            {
                var groupRows = new Rows();
                foreach (var e2 in e.GroupBy(x => x.Customer).OrderBy(x => x.Key.NameWithCode))
                {
                    var cells = new List<Cell>();
                    for (int i = 0; i < report.Periods.Length; i++)
                    {
                        var amount = e2.Where(x => x.Date >= report.Periods[i].FromDate && x.Date <= report.Periods[i].ToDate).Sum(x => x.AccountAmount);
                        cells.Add(Make(amount, new Link(new SalesInvoiceTotalsByCustomerTransactions { Business = Business, Referrer = Referrer, Customer = e2.Key.Key, From = report.Periods[i].FromDate, To = report.Periods[i].ToDate }.ToUrl())));
                    }

                    var allZero = cells.All(c => (c.Value ?? 0m) == 0m);
                    if (allZero) continue;

                    groupRows.Items.Add(new Row { Name = e2.Key.NameWithCode, Cells = cells });
                }

                // sort by first column descending
                groupRows.Items.Sort((a, b) => (b.Cells?[0].Value ?? 0m).CompareTo(a.Cells?[0].Value ?? 0m));

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
