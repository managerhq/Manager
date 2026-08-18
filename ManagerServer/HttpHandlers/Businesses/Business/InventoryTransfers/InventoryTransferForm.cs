using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryTransfers
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryTransfer), nameof(Strings.Edit))]
    [Guide("The `InventoryTransfer` form enables you to move inventory items between different locations within your business, maintaining accurate stock levels across all warehouses, stores, or storage facilities.")]
    [Header("Why Use Inventory Transfers")]
    [Guide("Inventory transfers are essential for businesses with multiple locations. They allow you to:")]
    [Guide("• Redistribute stock based on customer demand")]
    [Guide("• Consolidate inventory from multiple locations")]
    [Guide("• Fulfill orders from different warehouses")]
    [Guide("• Balance stock levels across your locations")]
    [Guide("The transfer process automatically decreases quantities at the source location and increases them at the destination. Your total inventory remains constant while accurately reflecting where items are physically located.")]
    [Header("Creating an Inventory Transfer")]
    [Guide("To create an inventory transfer:")]
    [Guide("1. Set the transfer date")]
    [Guide("2. Select the source location (where items are coming from)")]
    [Guide("3. Select the destination location (where items are going to)")]
    [Guide("4. Add line items for each inventory item being transferred")]
    [Guide("5. Specify the quantity to move for each item")]
    [Guide("You can include a reference number for tracking purposes and add descriptions or notes about the reason for the transfer. The system automatically updates inventory levels at both locations when you save the transfer.")]
    [Header("Form Fields")]
    [Guide("This form contains the following fields:")]
    [Fields(typeof(ManagerServer.Model.InventoryTransfer))]
    internal sealed class InventoryTransferForm : NakedVueForm<ManagerServer.Model.InventoryTransfer>
    {
        protected override bool CanHaveImage() => true;
    }
}