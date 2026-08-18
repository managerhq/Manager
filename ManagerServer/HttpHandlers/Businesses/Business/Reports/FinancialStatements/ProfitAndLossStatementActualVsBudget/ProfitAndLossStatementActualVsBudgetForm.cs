using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ProfitAndLossStatementActualVsBudget
{
    [ProtoContract]
    [Title(nameof(Strings.ProfitAndLossStatementActualVsBudget))]
    [Guide("The Profit and Loss Statement Actual vs Budget form configures budget comparison reports.")]
    [Guide("Set parameters to compare actual income and expenses against budgeted amounts.")]
    [Fields(typeof(ManagerServer.Model.ProfitAndLossStatementActualVsBudget))]
    internal sealed class ProfitAndLossStatementActualVsBudgetForm : NakedVueForm<ManagerServer.Model.ProfitAndLossStatementActualVsBudget>
    {
        protected override void OnSource(ManagerServer.Model.ProfitAndLossStatementActualVsBudget form, ManagerServer.Model.Object source)
        {
            if (source is ManagerServer.Model.ForecastProfitAndLossStatement report)
            {
                var items = ManagerServer.Api.Businesses.Business.Reports.ForecastProfitAndLossStatement.GetForecastProfitAndLossStatementView.GetItems(Business, report);
                form.FromDate = report.Periods[0].FromDate;
                form.ToDate = report.Periods[0].ToDate;
                form.ExcludeZeroBalances = report.ExcludeZeroBalances;
                form.Lines = items.Where(x => x.Amount != 0m).Select(x => new ManagerServer.Model.ProfitAndLossStatementActualVsBudget.BudgetItem() { Account = x.Account, Amount = x.Amount }).ToArray();
            }
        }
    }
}
