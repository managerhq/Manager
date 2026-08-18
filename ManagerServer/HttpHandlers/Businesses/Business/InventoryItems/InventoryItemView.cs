using System;
using ManagerServer.Api.Businesses.Business.InventoryItems;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryItems
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryItem))]
    [Guide("The *Inventory Item* view displays comprehensive information about a specific inventory item in your system.")]
    [Guide("This view provides a complete overview of the item's details, including its *code*, *name*, *description*, *unit of measurement*, and current pricing information.")]
    [Header("Available Actions")]
    [Guide("From this view, you can perform several actions:")]
    [Guide("• Click the **Edit** button to modify the item's details, such as name, description, or pricing")]
    [Guide("• View all transactions related to this inventory item to track its movement and usage")]
    [Guide("• Manage file attachments to store documents, images, or other files related to the item")]
    [Guide("• Access quantity information to see current *stock levels*, *items on order*, and *reserved quantities*")]
    [Header("Key Information")]
    [Guide("The view displays essential information such as the item's *purchase price* and *sale price*, helping you monitor profitability at a glance.")]
    [Guide("You can also see the item's current *quantity on hand* and track inventory movements through the transaction history.")]
    [LinkGuide("Learn more about editing inventory items:", typeof(InventoryItemForm))]
    internal sealed class InventoryItemView : DefaultView<GetInventoryItemView>
    {
    }
}
