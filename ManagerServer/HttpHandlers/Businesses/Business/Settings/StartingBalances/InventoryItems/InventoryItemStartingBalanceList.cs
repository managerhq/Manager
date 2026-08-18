using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Model.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.StartingBalances.InventoryItems
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(InventoryItems))]
    [Guid("470adecf-8f61-4a7b-aed7-e0056b1ba53c")]
    [Title(nameof(Strings.StartingBalances), nameof(Strings.InventoryItems))]
    [Guide("This screen allows you to set up starting balances for inventory items that you have created under the **Inventory Items** tab.")]
    [Guide("To create a new starting balance for an inventory item, click the **New Starting Balance** button.")]
    [HeroButtonScreenshot(title: nameof(Strings.InventoryItems), name: nameof(Strings.NewStartingBalance))]
    [Guide("You will be taken to the *Starting Balance* screen for the selected *Inventory Item*.")]
    [LinkGuide("For more information, see:", typeof(InventoryItemStartingBalanceForm))]
    internal sealed class InventoryItemStartingBalanceList : NakedObjectsWithAutomaticRows<InventoryItemStartingBalance>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewStartingBalance);
        }

        [Default]
        [Guid("dc26ce32-6384-4cb2-b7a8-27ff95c2d477")]
        public NamedObject[] GetInventoryItem(InventoryItemStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<InventoryItem>(x.InventoryItem)).ToArray();
        }

        [Default, Sum, Center]
        [Guid("3861b519-063e-476e-bd70-22a5cec511e4")]
        public decimal[] GetQtyOnHand(InventoryItemStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.QtyOnHandLines?.Sum(y => y.QtyOnHand) ?? 0m).ToArray();
        }

        [Default, Sum, Center]
        [Guid("362351c2-00c1-4b60-ab8b-03be0585cac5")]
        public decimal[] GetQtyToReceive(InventoryItemStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.QtyToReceiveLines?.Sum(y => y.QtyToReceive) ?? 0m).ToArray();
        }

        [Default, Sum, Center]
        [Guid("1551bf81-25f9-4354-acc4-0e273901c5a3")]
        public decimal[] GetQtyToDeliver(InventoryItemStartingBalance[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => x.QtyToDeliverLines?.Sum(y => y.QtyToDeliver) ?? 0m).ToArray();
        }
    }
}