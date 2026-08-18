using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryItems
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryItem), nameof(Strings.Edit))]
    [Guide("The inventory item edit form allows you to create a new inventory item or modify an existing one.")]
    [Guide("The form includes the following fields:")]
    [Fields(typeof(InventoryItem))]
    [Guide("You can add more details about this inventory item to meet your specific business needs by creating custom fields.")]
    [LinkGuide("For more information, see:", typeof(Settings.CustomFields.CustomFields))]
    [Header("Setting Up Starting Balances")]
    [Guide("Before setting up your inventory items, it is typically recommended to set up your customers and suppliers first.")]
    [Guide("This is because customers and suppliers can also have starting balances based on their unpaid invoices.")]
    [Guide("Here is the extended procedure to establish starting balances for your inventory items:")]
    [Header("Step 1: Enter Unpaid Invoices")]
    [Guide("`Suppliers`: Enter any unpaid purchase invoices for your suppliers. This will automatically adjust the `Qty Owned` for the inventory items purchased on these invoices.")]
    [Guide("`Customers`: Enter any unpaid sales invoices for your customers. This will automatically adjust the `Qty Owned` for the inventory items sold on these invoices.")]
    [Header("Step 2: Adjust for Historical Transactions")]
    [Guide("After entering unpaid invoices, use a journal entry to further adjust the `Qty Owned` for inventory items to account for historical purchases that won't be entered (typically because they are already paid invoices or other transaction types).")]
    [Header("Tracking Pending Deliveries and Receipts")]
    [Guide("If you are tracking `Qty to Deliver` and `Qty to Receive` columns, you can also establish starting balances for these columns:")]
    [Guide("`Qty to Deliver`: This represents the quantity that has been ordered by customers but has not yet been delivered. To establish this starting balance, create sales orders under the `Sales Orders` tab which haven't been fully delivered yet.")]
    [Guide("`Qty to Receive`: This represents the quantity that has been ordered from a supplier but has not yet been received. To establish this starting balance, create purchase orders under the `Purchase Orders` tab which haven't been fully delivered yet.")]
    [Guide("By following these steps, you ensure that your inventory balances are accurately reflected, including adjustments for unpaid invoices and historical purchases.")]
    internal sealed class InventoryItemForm : NakedVueForm<ManagerServer.Model.InventoryItem>
    {
        protected override bool CanHaveImage() => true;
    }
}
