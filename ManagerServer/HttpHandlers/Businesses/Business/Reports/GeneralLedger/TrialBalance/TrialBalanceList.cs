using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.TrialBalance
{
    [ProtoContract]
    [Title(nameof(Strings.TrialBalance))]
    [NewButton(nameof(Strings.NewReport))]
    [Guide("`TrialBalance` a vital tool that provides a snapshot of your business's financial performance and position by listing all ledger account balances and ensuring that debits and credits are balanced.")]
    [Guide("To create a new `TrialBalance`, go to `Reports` tab, click `TrialBalance`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.TrialBalance), name: nameof(Strings.NewReport))]
    internal sealed class TrialBalanceList : PersistentObjectTable<ManagerServer.Model.TrialBalance>
    {
        [MinWidth, Center, WhitespaceNoWrap]
        [Guid("1552dd52-3b24-488b-a205-5df61ad41b3a")]
        public DateTime? GetFromDate(ManagerServer.Model.TrialBalance o) => o.Periods?[0].FromDate;

        [MinWidth, Center, WhitespaceNoWrap]
        [Guid("278445f8-79e1-452f-a775-b9ed33c2becb")]
        public DateTime? GetToDate(ManagerServer.Model.TrialBalance o) => o.Periods?[0].ToDate;

        [MinWidth, Center, WhitespaceNoWrap]
        [Guid("2cf6f96c-c15f-4cf0-bd21-6fbe4c37e334")]
        public AccountingBasis GetAccountingMethod(ManagerServer.Model.TrialBalance o) => o.AccountingMethod;

        [Guid("4070ebfe-7611-4238-952d-dd1d261904e5")]
        public string GetDescription(ManagerServer.Model.TrialBalance o) => o.Description;       
    }
}