using ManagerServer.Globalization;
using ManagerServer.Helpers;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.Reports.EmployeeSummary
{
    [ProtoContract]
    internal sealed class GetEmployeeSummaryView : GetReportView<Model.EmployeeSummary>
    {
        protected override string DefaultTitle => Strings.EmployeeSummary;

        protected override ReportModel Build(Database business, Model.EmployeeSummary report)
        {
            var model = new ReportModel();

            var employee = business.SingleOrDefault<ManagerServer.Model.Employee>(report.Employee);
            if (employee == null) return model;

            model.Subtitle = employee.Name;
            model.Subtitle2 = string.Format(Strings.For_the_period_from_XXX_to_XXX, report.FromDate.ToLocalShortDisplayString(), report.ToDate.ToLocalShortDisplayString());

            model.Columns.Add(new Column { Key = "total", Name = Strings.Total });

            Cell Make(decimal? v, Link link = null) => ReportNumberFormat.Cell(v, NumberStyle.Currency, model.WholeNumbers, link);

            var currency = employee.Currency;
            var currencies = ManagerServer.Query.Currencies.GetCurrencyProvider(Business);
            var decimalDigits = currencies.Get(currency).GetDecimalPlaces();

            var payslips = business.OfType<ManagerServer.Model.Payslip>().Where(x => x.employee == employee.Key && x.Date >= report.FromDate && x.Date <= report.ToDate).ToArray();

            var payslipEarningsItems = business.OfType<ManagerServer.Model.PayslipEarningsItem>().OrderBy(x => x.Name).ToArray();
            var payslipDeductionItems = business.OfType<ManagerServer.Model.PayslipDeductionItem>().OrderBy(x => x.Name).ToArray();
            var payslipContributionItems = business.OfType<ManagerServer.Model.PayslipContributionItem>().OrderBy(x => x.Name).ToArray();

            var earningsRows = new Rows { TotalText = Strings.GrossPay };
            foreach (var e in payslipEarningsItems)
            {
                var total = payslips.SelectMany(x => x.Earnings ?? new ManagerServer.Model.Payslip.Earned[0]).Where(x => x.Item == e.Key).Sum(x => System.Math.Round((x.Units ?? 1m) * x.UnitPrice, decimalDigits, System.MidpointRounding.AwayFromZero));
                earningsRows.Items.Add(new Row { Name = e.Name, Cells = new System.Collections.Generic.List<Cell> { Make(total) } });
            }

            var deductionsRows = new Rows { TotalText = Strings.TotalDeductions, IsLess = true };
            foreach (var e in payslipDeductionItems)
            {
                var total = payslips.SelectMany(x => x.Deductions ?? new ManagerServer.Model.Payslip.Deduction[0]).Where(x => x.Item == e.Key).Sum(x => System.Math.Round(x.DeductionAmount, decimalDigits, System.MidpointRounding.AwayFromZero));
                deductionsRows.Items.Add(new Row { Name = e.Name, Cells = new System.Collections.Generic.List<Cell> { Make(total * -1m) } });
            }

            var contributionsRows = new Rows { TotalText = Strings.TotalContributions };
            foreach (var e in payslipContributionItems)
            {
                var total = payslips.SelectMany(x => x.Contributions ?? new ManagerServer.Model.Payslip.Contribution[0]).Where(x => x.Item == e.Key).Sum(x => System.Math.Round(x.ContributionAmount, decimalDigits, System.MidpointRounding.AwayFromZero));
                contributionsRows.Items.Add(new Row { Name = e.Name, Cells = new System.Collections.Generic.List<Cell> { Make(total) } });
            }

            model.Rows.Items.Add(new Row { Name = Strings.PayslipEarningsItems, Rows = earningsRows });
            model.Rows.Items.Add(new Row { Name = Strings.PayslipDeductionItems, Rows = deductionsRows });
            model.Rows.Items.Add(new Row { IsTotalRow = true, Name = Strings.NetPay });
            model.Rows.Items.Add(new Row { Name = Strings.PayslipContributionItems, Rows = contributionsRows });

            model.Prune(report.ExcludeZeroBalances);

            return model;
        }
    }
}
