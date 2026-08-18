using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.InventoryKits
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("3f1cb42c-7812-4d4d-b6ed-c85b809ae763")]
    [IfTab(nameof(InventoryItems))]
    [Title(nameof(Strings.InventoryKits))]
    [Guide("The **Inventory Kits** screen can be found under the **Settings** tab.")]
    [SettingsItemScreenshot("fa-box-full", nameof(Strings.InventoryKits))]
    [Guide("An *inventory kit* is essentially a bundle of *inventory items* that are sold as a package, but aren't physically grouped or stored as a single unit. The items within the kit can also be sold individually at different times. When a kit is sold, its components are collected from their respective storage locations for shipment. While the inventory kit isn't used for manufacturing, it serves as a convenient sales strategy.")]
    [Header("Advantages of Inventory Kits")]
    [Guide("Using inventory kits provides several key benefits:")]
    [Guide("• Reduces the time it takes to enter transactions")]
    [Guide("• Establishes consistent pricing (including discounts or premiums) for items that are sold as a bundle")]
    [Guide("• Eliminates the need to pre-assemble kits")]
    [Guide("• Removes the need to forecast demand for kit sales compared to component sales")]
    [Header("Creating an Inventory Kit")]
    [Guide("To create an inventory kit, you must first create each item in it as individual inventory items.")]
    [LinkGuide("For more information, see:", typeof(InventoryItems.InventoryItems))]
    [Guide("To create a new inventory kit, click the **New Inventory Kit** button.")]
    [HeroButtonScreenshot(nameof(Strings.InventoryKits), nameof(Strings.NewInventoryKit))]
    //[LinkGuide("For more information see:", typeof(InventoryKitForm))]
    [Guide("Once a kit is defined, it functions the same as an inventory item in sales-related transactions. However, it doesn't need to be counted since it doesn't exist as separate stock. Only the components are treated as physical inventory.")]
    internal sealed class InventoryKits : NakedObjectsWithAutomaticRows<ManagerServer.Model.InventoryKit>
    {
        [Guid("ad30f123-7a4d-4016-8c70-613cff15f4c7")]
        public string[] GetItemCode(ManagerServer.Model.InventoryKit[] rows)
        {
            return rows.Select(x => x.ItemCode).ToArray();
        }

        [Default]
        [Guid("223dcdce-6d77-4bc7-befa-38fad431720f")]
        public string[] GetItemName(ManagerServer.Model.InventoryKit[] rows)
        {
            return rows.Select(x => x.ItemName).ToArray();
        }

        [Guid("6aa6e2f3-af51-4cf1-93fb-824272790ffd")]
        public string[] GetUnitName(ManagerServer.Model.InventoryKit[] rows)
        {
            return rows.Select(x => x.UnitName).ToArray();
        }

        [Default]
        [Right]
        [Guid("c6c39e7c-1d81-4115-ae47-3e0ca90ab003")]
        public decimal?[] GetSalesPrice(ManagerServer.Model.InventoryKit[] rows)
        {
            return rows.Select(x => x.HasDefaultSalesUnitPrice ? (decimal?)x.DefaultSalesUnitPrice : null).ToArray();
        }
    }
}
