using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ChartOfAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.Account), nameof(Strings.InventoryCost))]
    [Guide("This form allows to rename built-in `InventoryCost` account.")]
    [Guide("To access this form, go to `Settings`, then `ChartOfAccounts`, then click `Edit` button for `InventoryCost` account.")]
    [Guide("The form contains the following fields:")]
    [Fields(typeof(ProfitAndLossStatementAccountInventoryPurchases))]
    [Guide("Click `Update` button to save your changes.")]
    [Guide("This account cannot be deleted, it is automatically added to your `ChartOfAccounts` when you have at least one inventory item.")]
    [LinkGuide("For more information see:", typeof(InventoryItems.InventoryItems))]
    internal sealed class ProfitAndLossStatementAccountInventoryPurchasesForm : NakedVueForm<ProfitAndLossStatementAccountInventoryPurchases>
    {
    }
}