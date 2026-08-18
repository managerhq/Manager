using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.InventoryItems
{
    [ProtoContract]
    [Title(nameof(Strings.StartingBalance), nameof(Strings.InventoryItems), nameof(Strings.Edit))]
    [Guide("This form is the place where you can set up starting balance for inventory item.")]
    [Guide("To access this form, go to `Settings` tab, then `StartingBalances`, then `InventoryItems`.")]
    [Guide("The form includes the following fields:")]
    [Fields(typeof(InventoryItemStartingBalance))]
    internal sealed class InventoryItemStartingBalanceForm : NakedVueForm<InventoryItemStartingBalance>
    {
        protected override bool CanHaveImage() => true;
    }
}