using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Payslips
{
    [ProtoContract]
    internal sealed class GetPayslipView : GetTransactionView<Model.Payslip>
    {
        protected override TransactionView GetViewData(Model.Payslip o)
        {
            var viewData = new TransactionView();

            if (!o.employee.HasValue) return viewData;
            var employee = Database.SingleOrDefault<Model.Employee>(o.employee.Value);
            if (employee == null) return viewData;

            var payslipEarningItems = Database.OfType<Model.PayslipEarningsItem>().ToDictionary(x => x.Key);
            var payslipDeductionItems = Database.OfType<Model.PayslipDeductionItem>().ToDictionary(x => x.Key);
            var payslipContributionItems = Database.OfType<Model.PayslipContributionItem>().ToDictionary(x => x.Key);

            var currencies = Query.Currencies.GetCurrencyProvider(Business);
            var currency = currencies.Get(employee.Currency);

            viewData.title = Strings.Payslip;
            if (o.HasPayslipCustomTitle && !string.IsNullOrWhiteSpace(o.PayslipCustomTitle)) viewData.title = o.PayslipCustomTitle;
            viewData.reference = o.Reference;

            viewData.recipient.name = employee.Name;
            viewData.recipient.address = employee.Address;
            viewData.recipient.email = employee.Email;

            viewData.fields.Add(new TransactionView.Field { label = Strings.Date, text = o.Date.ToLocalShortDisplayString() });
            if (!string.IsNullOrWhiteSpace(o.Reference)) viewData.fields.Add(new TransactionView.Field { label = Strings.Reference, text = o.Reference });

            viewData.description = o.description;

            var qty = o.Earnings != null && o.Earnings.Any(x => x.Units.HasValue);

            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Description });
            if (qty)
            {
                viewData.table.columns.Add(new TransactionView.Column { label = Strings.Qty, align = "right", nowrap = true });
                viewData.table.columns.Add(new TransactionView.Column { label = Strings.Rate, align = "right", nowrap = true });
            }
            viewData.table.columns.Add(new TransactionView.Column { label = Strings.Total, align = "right", nowrap = true });

            var total = 0m;
            if (o.Earnings != null)
            {
                foreach (var e in o.Earnings)
                {
                    var row = new TransactionView.Row();

                    var description = Strings.Earnings;
                    if (!string.IsNullOrWhiteSpace(e.Description)) description = e.Description;
                    else if (e.Item.HasValue && payslipEarningItems.ContainsKey(e.Item.Value)) description = payslipEarningItems[e.Item.Value].Name;
                    row.cells.Add(new TransactionView.Cell { text = description });

                    if (qty)
                    {
                        row.cells.Add(new TransactionView.Cell { text = e.Units.ToNumberString() });
                        row.cells.Add(new TransactionView.Cell { text = e.UnitPrice.ToCurrencyString(currency: currency, currencySymbol: CurrencySymbol.None) });
                    }

                    var lineTotal = Math.Round((e.Units ?? 1m) * e.UnitPrice, currency.GetDecimalPlaces(), MidpointRounding.AwayFromZero);
                    total += lineTotal;

                    row.cells.Add(new TransactionView.Cell { text = lineTotal != 0 ? lineTotal.ToCurrencyString(currency, currencySymbol: CurrencySymbol.None) : null });

                    viewData.table.rows.Add(row);
                }
            }

            viewData.table.totals.Add(new TransactionView.Total { label = Strings.GrossPay, text = total.ToCurrencyString(currency, CurrencySymbol.Short), number = total });

            if (o.Deductions != null)
            {
                foreach (var e in o.Deductions)
                {
                    if (e.DeductionAmount == 0m) continue;
                    var description = Strings.Less + ": " + Strings.Deduction;
                    if (!string.IsNullOrWhiteSpace(e.Description)) description = Strings.Less + ": " + e.Description;
                    else if (e.Item.HasValue && payslipDeductionItems.ContainsKey(e.Item.Value)) description = Strings.Less + ": " + payslipDeductionItems[e.Item.Value].Name;

                    viewData.table.totals.Add(new TransactionView.Total { label = description, text = e.DeductionAmount.ToCurrencyString(currency, CurrencySymbol.Short), number = e.DeductionAmount });

                    total -= e.DeductionAmount;
                }
            }

            viewData.table.totals.Add(new TransactionView.Total { label = Strings.NetPay, text = total.ToCurrencyString(currency, CurrencySymbol.Short), number = total, emphasis = true });

            if (o.Contributions != null)
            {
                foreach (var e in o.Contributions)
                {
                    if (e.ContributionAmount == 0m) continue;
                    var description = Strings.EmployerContribution;
                    if (!string.IsNullOrWhiteSpace(e.Description)) description = e.Description;
                    else if (e.Item.HasValue && payslipContributionItems.ContainsKey(e.Item.Value)) description = payslipContributionItems[e.Item.Value].Name;

                    viewData.table.totals.Add(new TransactionView.Total { label = description, text = e.ContributionAmount.ToCurrencyString(currency, CurrencySymbol.Short), number = e.ContributionAmount });
                }
            }

            if (o.ShowTotalsForThePeriod && o.TotalsPeriodStart.HasValue)
            {
                var label = string.Format(Strings.For_the_period_from_XXX_to_XXX, o.TotalsPeriodStart.Value.ToLocalShortDisplayString(), o.Date.ToLocalShortDisplayString());

                var output = @"<div><b>"+label+"</b></div>";
                output += @"<table>";
                var ytdEarnings = Database.OfType<Model.Payslip>().Where(x => x.employee == o.employee && x.Date >= o.TotalsPeriodStart.Value && x.Date <= o.Date && x.Earnings != null).SelectMany(x => x.Earnings).Where(x => x.Item.HasValue && x.UnitPrice != 0m).GroupBy(x => x.Item.Value).ToArray();
                foreach (var e in ytdEarnings)
                {
                    if (!payslipEarningItems.ContainsKey(e.Key)) continue;
                    output += "<tr>";
                    output += @"<td><div style=""white-space: nowrap; padding: 3px 0px"">" + Strings.Total + " &mdash; " + payslipEarningItems[e.Key].Name + "</div></td>";
                    output += @"<td style=""text-align: right; padding: 3px 0px"">" + e.Sum(x => Math.Round((x.Units ?? 1m) * x.UnitPrice, currency.GetDecimalPlaces(), MidpointRounding.AwayFromZero)).ToCurrencyString(currency, CurrencySymbol.Short) + "</td>";
                    output += "</tr>";
                }
                var ytdDeductions = Database.OfType<Model.Payslip>().Where(x => x.employee == o.employee && x.Date >= o.TotalsPeriodStart.Value && x.Date <= o.Date && x.Deductions != null).SelectMany(x => x.Deductions).Where(x => x.Item.HasValue && x.DeductionAmount != 0m).GroupBy(x => x.Item.Value).ToArray();
                foreach (var e in ytdDeductions)
                {
                    if (!payslipDeductionItems.ContainsKey(e.Key)) continue;
                    output += "<tr>";
                    output += @"<td><div style=""white-space: nowrap; padding: 3px 0px"">" + Strings.Total + " &mdash; " + payslipDeductionItems[e.Key].Name + "</div></td>";
                    output += @"<td style=""text-align: right; padding: 3px 0px"">" + e.Sum(x => x.DeductionAmount).ToCurrencyString(currency, CurrencySymbol.Short) + "</td>";
                    output += "</tr>";
                }
                var ytdContributions = Database.OfType<Model.Payslip>().Where(x => x.employee == o.employee && x.Date >= o.TotalsPeriodStart.Value && x.Date <= o.Date && x.Contributions != null).SelectMany(x => x.Contributions).Where(x => x.Item.HasValue && x.ContributionAmount != 0m).GroupBy(x => x.Item.Value).ToArray();
                foreach (var e in ytdContributions)
                {
                    if (!payslipContributionItems.ContainsKey(e.Key)) continue;
                    output += "<tr>";
                    output += @"<td><div style=""white-space: nowrap; padding: 3px 0px"">" + Strings.Total + " &mdash; " + payslipContributionItems[e.Key].Name + "</div></td>";
                    output += @"<td style=""text-align: right; padding: 3px 0px"">" + e.Sum(x => x.ContributionAmount).ToCurrencyString(currency, CurrencySymbol.Short) + "</td>";
                    output += "</tr>";
                }
                output += "</table>";                
                viewData.footers = [ output ];
            }

            viewData.custom_fields.AddRange(GetCustomFields(typeof(Model.Employee), employee.CustomFields));

            return viewData;
        }
    }
}
