using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.BalanceSheetByGroup
{
    [ProtoContract]
    [Title(nameof(Strings.BalanceSheetByGroup))]
    [Guide("The `BalanceSheetByGroup` report shows the contents of a single balance sheet group, including its accounts and nested subgroups. It is useful when the main `BalanceSheet` report has groups collapsed and you need a separate report to see the detail of one of those groups.")]
    [Guide("To create a new `BalanceSheetByGroup` report, go to `Reports` tab, click `BalanceSheetByGroup`, then `NewReport` button.")]
    [HeroButtonScreenshot(title: nameof(Strings.BalanceSheetByGroup), name: nameof(Strings.NewReport))]
    [NewButton(nameof(Strings.NewReport))]
    internal sealed class BalanceSheetByGroupList : PersistentObjectTable<ManagerServer.Model.BalanceSheetByGroup>
    {
        [MinWidth, WhitespaceNoWrap, Center]
        [Guid("4134500e-938b-40b3-8ece-4d63915b90b4")]
        public DateTime? GetDate(ManagerServer.Model.BalanceSheetByGroup row) => row.Periods?[0]?.Date;

        [Guid("ee209e28-b328-4b1b-a19e-eb33a7fafaf5")]
        public NamedObject GetGroup(ManagerServer.Model.BalanceSheetByGroup row)
        {
            if (!row.Group.HasValue) return null;
            var group = ApplicationData.Businesses.Get(Business).Single(row.Group.Value) as BalanceSheetAbstractGroup;
            if (group != null) return group;
            group = ApplicationData.Businesses.Get(Business).SingleOrDefault<BalanceSheetAbstractGroup>(row.Group.Value);
            if (group != null) return group;
            return null;
        }

        [Guid("7b61dc10-40d2-480d-ac99-6729b12defe0")]
        public AccountingBasis GetAccountingMethod(ManagerServer.Model.BalanceSheetByGroup row) => row.AccountingMethod;
    }
}
