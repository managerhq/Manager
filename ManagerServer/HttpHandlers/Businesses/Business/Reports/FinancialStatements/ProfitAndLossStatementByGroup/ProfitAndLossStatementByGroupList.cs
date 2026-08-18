using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.ProfitAndLossStatementByGroup
{
    [ProtoContract]
    [Title(nameof(Strings.ProfitAndLossStatementByGroup))]
    [Guide("The `ProfitAndLossStatementByGroup` report shows the contents of a single profit & loss group, including its accounts and nested subgroups. It is useful when the main `ProfitAndLossStatement` report has groups collapsed and you need a separate report to see the detail of one of those groups.")]
    [Guide("To create a new `ProfitAndLossStatementByGroup` report, go to `Reports` tab, click `ProfitAndLossStatementByGroup`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.ProfitAndLossStatementByGroup), name: nameof(Strings.NewReport))]
    [NewButton(nameof(Strings.NewReport))]
    internal sealed class ProfitAndLossStatementByGroupList : PersistentObjectTable<ManagerServer.Model.ProfitAndLossStatementByGroup>
    {
        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("a2eaef07-44a1-4d10-9c4b-2b8a2dc9c14e")]
        public DateTime? GetFromDate(ManagerServer.Model.ProfitAndLossStatementByGroup row) => row.Periods?[0]?.FromDate;

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("1f7e2c1c-7d4e-4b54-bf63-7c5b8ad6d0e3")]
        public DateTime? GetToDate(ManagerServer.Model.ProfitAndLossStatementByGroup row) => row.Periods?[0]?.ToDate;

        [Guid("9c8d0e3a-13c4-4bca-9a47-0a9a4f6f6b71")]
        public NamedObject GetGroup(ManagerServer.Model.ProfitAndLossStatementByGroup row)
        {
            if (!row.Group.HasValue) return null;
            var group = ApplicationData.Businesses.Get(Business).SingleOrDefault<ProfitAndLossStatementGroup>(row.Group.Value);
            if (group != null) return group;
            return null;
        }

        [Guid("4e9b5d77-9a0b-4d7e-8ab5-2ad6f4c2e5d4")]
        public AccountingBasis GetAccountingMethod(ManagerServer.Model.ProfitAndLossStatementByGroup row) => row.AccountingMethod;
    }
}
