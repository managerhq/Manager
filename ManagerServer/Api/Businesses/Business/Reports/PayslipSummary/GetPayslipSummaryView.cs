using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.PayslipSummary
{
    [ProtoContract]
    internal sealed class GetPayslipSummaryView : GetReportView<Model.PayslipSummary>
    {
        protected override string DefaultTitle => Strings.PayslipSummary;

        protected override ReportModel Build(Database business, Model.PayslipSummary report)
        {
            var model = new ReportModel();
            model.Subtitle = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            var employees = business.OfType<ManagerServer.Model.Employee>().ToDictionary(x => x.Key);

            model.Columns.Add(new Column { Name = Strings.GrossPay });
            model.Columns.Add(new Column { Name = Strings.TotalDeductions });
            model.Columns.Add(new Column { Name = Strings.NetPay, IsBold = true });
            model.Columns.Add(new Column { Name = Strings.TotalContributions });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.Currency, model.WholeNumbers, link);

            var payslips = business.OfType<ManagerServer.Model.Payslip>().Where(x => x.employee.HasValue && employees.ContainsKey(x.employee.Value) && x.Date >= report.FromDate && x.Date <= report.ToDate).GroupBy(x => x.employee.Value).ToDictionary(x => x.Key, x => x.ToArray());
            var currencies = ManagerServer.Query.Currencies.GetCurrencyProvider(Business);

            // Collect all currency groups
            var currencyGroups = employees.Values.GroupBy(x => currencies.Get(x.Currency)).ToArray();

            var groupRowsList = new List<Row>();

            foreach (var currency in currencyGroups)
            {
                var groupRows = new Rows { MakeTotalStandOut = true };

                foreach (var e in currency.OrderBy(x => x.Name))
                {
                    if (!payslips.ContainsKey(e.Key)) continue;

                    var grossPay = payslips[e.Key].Where(x => x.Earnings != null).Sum(x => x.Earnings.Sum(y => System.Math.Round((y.Units ?? 1m) * y.UnitPrice, currency.Key.GetDecimalPlaces(), System.MidpointRounding.AwayFromZero)));
                    var deductions = payslips[e.Key].Where(x => x.Deductions != null).Sum(x => x.Deductions.Sum(y => y.DeductionAmount));
                    var contributions = payslips[e.Key].Where(x => x.Contributions != null).Sum(x => x.Contributions.Sum(y => y.ContributionAmount));
                    var netPay = grossPay - deductions;

                    var cells = new List<Cell>
                    {
                        Make(grossPay),
                        Make(deductions),
                        Make(netPay),
                        Make(contributions),
                    };

                    // ExcludeIfZero: skip if all zero
                    if (cells.All(c => (c.Value ?? 0m) == 0m)) continue;

                    groupRows.Items.Add(new Row { Name = e.NameWithCode, Cells = cells });
                }

                groupRowsList.Add(new Row { Name = currency.Key.GetDisplayName(), Rows = groupRows });
            }

            if (groupRowsList.Count == 1)
            {
                // Single currency: unwrap group rows directly
                foreach (var r in groupRowsList[0].Rows.Items)
                {
                    model.Rows.Items.Add(r);
                }
                model.Rows.Items.Add(new Row { IsTotalRow = true });
            }
            else
            {
                foreach (var g in groupRowsList)
                {
                    model.Rows.Items.Add(g);
                }
            }

            return model;
        }
    }
}
