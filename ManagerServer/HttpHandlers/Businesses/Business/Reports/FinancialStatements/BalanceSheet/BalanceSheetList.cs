using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.BalanceSheet
{
    [ProtoContract]
    [Title(nameof(Strings.BalanceSheet))]
    [Guide("The `BalanceSheet` provides a snapshot of your business's financial position at a specific point in time, detailing assets, liabilities, and equity to help you assess financial health.")]
    [Guide("To create a new `BalanceSheet`, go to `Reports` tab, click `BalanceSheet`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.BalanceSheet), name: nameof(Strings.NewReport))]
    [NewButton(nameof(Strings.NewReport))]
    internal sealed class BalanceSheetList : PersistentObjectTable<ManagerServer.Model.BalanceSheet>
    {
        [MinWidth, WhitespaceNoWrap, Center]
        [Guid("1150c61a-8676-43c9-82dd-67c3109da597")]
        public DateTime? GetDate(ManagerServer.Model.BalanceSheet row) => row.Periods?[0]?.Date;

        [HideColumnIfAllEmpty]
        [Guid("9c52179c-be01-42e2-bbbe-99194a188296")]
        public string GetDescription(ManagerServer.Model.BalanceSheet row) => row.Description;

        [Guid("90cb3ecb-6916-4272-bf0b-03eb1e1ad6f1")]
        public AccountingBasis GetAccountingMethod(ManagerServer.Model.BalanceSheet row) => row.AccountingMethod;
    }
}