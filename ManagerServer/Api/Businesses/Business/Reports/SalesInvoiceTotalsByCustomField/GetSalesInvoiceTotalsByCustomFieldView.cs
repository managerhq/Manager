using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.SalesInvoiceTotalsByCustomField;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.SalesInvoiceTotalsByCustomField
{
    [ProtoContract]
    internal sealed class GetSalesInvoiceTotalsByCustomFieldView : GetReportView<Model.SalesInvoiceTotalsByCustomField>
    {
        protected override string DefaultTitle => Strings.SalesInvoiceTotalsByCustomField;

        protected override ReportModel Build(Database business, Model.SalesInvoiceTotalsByCustomField report)
        {
            var model = new ReportModel();
            model.Title = report.Name ?? DefaultTitle;
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.Periods[0].FromDate.ToLocalShortDisplayString(), report.Periods[0].ToDate.ToLocalShortDisplayString());

            for (int i = 0; i < report.Periods.Length; i++)
            {
                var columnName = report.Periods[i].ToDate.ToLocalShortDisplayString();
                if (!string.IsNullOrWhiteSpace(report.Periods[i].ColumnName)) columnName = report.Periods[i].ColumnName;
                model.Columns.Add(new Column { Name = columnName, IsBold = (i == 0) });
            }

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.CurrencyParentheses, model.WholeNumbers, link);

            var salesInvoices = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => (x.Transaction is SalesInvoice || x.Transaction is CreditNote)).ToArray();

            var transactions = new Tuple<DateTime, string, decimal>[0];

            var customField = business.SingleOrDefault<CustomField>(report.CustomField);

            if (customField != null)
            {
                if (customField.Contains(typeof(SalesInvoice)))
                {
                    transactions = salesInvoices.Where(x => x.IsBalancing && x.SalesInvoice?.CustomFields != null && x.SalesInvoice.CustomFields.ContainsKey(customField.Key)).Select(x => new Tuple<DateTime, string, decimal>(x.Date, x.SalesInvoice.CustomFields[customField.Key], x.BaseAmount * -1m)).ToArray();
                }
                if (customField.Contains(typeof(SalesInvoice.Line)))
                {
                    transactions = salesInvoices.Where(x => x.TransactionLine?.GetCustomFields() != null && x.TransactionLine.GetCustomFields().ContainsKey(customField.Key)).Select(x => new Tuple<DateTime, string, decimal>(x.Date, x.TransactionLine.GetCustomFields()[customField.Key], x.BaseAmount)).ToArray();
                }
                if (customField.Contains(typeof(Customer)))
                {
                    transactions = salesInvoices.Where(x => x.IsBalancing && x.Customer?.CustomFields != null && x.Customer.CustomFields.ContainsKey(customField.Key)).Select(x => new Tuple<DateTime, string, decimal>(x.Date, x.Customer.CustomFields[customField.Key], x.BaseAmount * -1m)).ToArray();
                }
                if (customField.Contains(typeof(InventoryItem)))
                {
                    transactions = salesInvoices.Where(x => x.InventoryItem?.CustomFields != null && x.InventoryItem.CustomFields.ContainsKey(customField.Key)).Select(x => new Tuple<DateTime, string, decimal>(x.Date, x.InventoryItem.CustomFields[customField.Key], x.BaseAmount)).ToArray();
                }
                if (customField.Contains(typeof(NonInventoryItem)))
                {
                    transactions = salesInvoices.Where(x => x.NonInventoryItem?.CustomFields != null && x.NonInventoryItem.CustomFields.ContainsKey(customField.Key)).Select(x => new Tuple<DateTime, string, decimal>(x.Date, x.NonInventoryItem.CustomFields[customField.Key], x.BaseAmount)).ToArray();
                }

                foreach (var e2 in transactions.GroupBy(x => x.Item2).OrderBy(x => x.Key))
                {
                    var cells = new List<Cell>();
                    for (int i = 0; i < report.Periods.Length; i++)
                    {
                        var amount = e2.Where(x => x.Item1 >= report.Periods[i].FromDate && x.Item1 <= report.Periods[i].ToDate).Sum(x => x.Item3) * -1m;
                        cells.Add(Make(amount, new Link(new SalesInvoiceTotalsByCustomFieldTransactions { Business = Business, Referrer = Referrer, From = report.Periods[i].FromDate, To = report.Periods[i].ToDate, CustomField = customField.Key, Value = e2.Key }.ToUrl())));
                    }

                    var allZero = cells.All(c => (c.Value ?? 0m) == 0m);
                    if (allZero) continue;

                    model.Rows.Items.Add(new Row { Name = e2.Key, Cells = cells });
                }

                model.Rows.Items.Add(new Row { IsTotalRow = true });
            }

            return model;
        }
    }
}
