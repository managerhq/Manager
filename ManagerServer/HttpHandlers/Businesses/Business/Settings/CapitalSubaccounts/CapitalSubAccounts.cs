using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CapitalSubaccounts
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(CapitalAccounts))]
    [Title(nameof(Strings.CapitalSubaccounts))]
    [NewButton(nameof(Strings.NewSubaccount))]
    [Guide("Capital subaccounts enable you to categorize transactions within each capital account for better tracking and reporting.")]
    [SettingsItemScreenshot("fa-list", nameof(Strings.CapitalSubaccounts))]
    [Guide("This feature allows you to group capital account transactions into categories such as *Drawings*, *Funds Contributed*, *Share of Profit*, and more. Subaccounts created here are available for all capital accounts in your business.")]
    [Guide("When entering transactions in any capital control account, you will first select the capital account from the **Capital Accounts** tab, then choose one of the subaccounts defined on this screen.")]
    [Guide("To view capital account movements organized by both accounts and subaccounts, go to the **Reports** tab and select **Capital Accounts Summary**.")]
    internal sealed class CapitalSubaccounts : PersistentObjectTable<ManagerServer.Model.SubAccount>
    {
        [Guid("bc610b7e-cbf8-4cd7-8a65-06df5e9d14c5")]
        public string GetName(ManagerServer.Model.SubAccount o) => o.Name;
    }
}
