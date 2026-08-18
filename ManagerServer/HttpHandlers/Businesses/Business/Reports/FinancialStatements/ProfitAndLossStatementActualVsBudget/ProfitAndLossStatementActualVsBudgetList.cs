using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ProfitAndLossStatementActualVsBudget
{
    [ProtoContract]
    [Title(nameof(Strings.ProfitAndLossStatementActualVsBudget))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`ProfitAndLossStatementActualVsBudget` report provides a detailed comparison between your company's actual financial performance and the budgeted figures, offering valuable insights into variances and helping you make informed financial decisions.")]
    [Guide("To create a new `ProfitAndLossStatementActualVsBudget`, go to `Reports` tab, click `ProfitAndLossStatementActualVsBudget`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.ProfitAndLossStatementActualVsBudget), name: nameof(Strings.NewReport))]
    internal sealed class ProfitAndLossStatementActualVsBudgetList : PersistentObjectTable<ManagerServer.Model.ProfitAndLossStatementActualVsBudget>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("cd650df8-092a-4473-af88-83d5217a8a48")]
        public DateTime GetFromDate(ManagerServer.Model.ProfitAndLossStatementActualVsBudget o) => o.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("4cf142da-5b93-454c-9202-d2a969edd676")]
        public DateTime GetToDate(ManagerServer.Model.ProfitAndLossStatementActualVsBudget o) => o.ToDate;

        [Guid("4cca932f-9a8c-4d1a-b8b0-39dbcb8da6d5")]
        public AccountingBasis GetAccountingMethod(ManagerServer.Model.ProfitAndLossStatementActualVsBudget o) => o.AccountingMethod;

        [HideColumnIfAllEmpty]
        [Guid("6870c949-9196-4e58-a2fb-db44578e615f")]
        public ManagerServer.Model.Division GetDivision(ManagerServer.Model.ProfitAndLossStatementActualVsBudget o) => ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Division>(o.Division);

        [Guid("921e0643-4f22-4efd-a339-f0ef035a0c50")]
        public string GetTitle(ManagerServer.Model.ProfitAndLossStatementActualVsBudget o) => o.Title.IfEmptyReplaceWith(Strings.ProfitAndLossStatementActualVsBudget);
    }
}