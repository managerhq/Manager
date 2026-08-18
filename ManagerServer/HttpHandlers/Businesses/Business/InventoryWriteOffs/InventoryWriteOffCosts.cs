using ManagerServer.HttpHandlers.Businesses.Business.InventoryItems;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryWriteOffs
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryWriteOffs))]
    [Guide("The **Inventory Write-Off Costs** screen displays the financial impact of writing off inventory items from your records.")]
    [Guide("This screen shows the actual cost values that will be removed from your *inventory asset accounts* when items are written off.")]
    [Header("Understanding Write-Off Costs")]
    [Guide("When you write off inventory items due to damage, theft, obsolescence, or other losses, the system calculates the cost impact based on your *inventory valuation method* (FIFO, LIFO, or average cost).")]
    [Guide("The costs displayed represent the original purchase or production value of the items being written off. This amount will be credited from your inventory asset account and debited to your designated write-off expense account.")]
    [Header("Using This Information")]
    [Guide("Review these costs before confirming the write-off to understand the financial impact on your accounts.")]
    [Guide("This information helps you track inventory losses, make informed decisions about inventory management, and maintain accurate financial records.")]
    internal sealed class InventoryWriteOffCosts : TransactionCosts
    {
    }
}
