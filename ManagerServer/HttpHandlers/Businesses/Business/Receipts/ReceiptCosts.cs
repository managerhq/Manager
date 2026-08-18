using ManagerServer.HttpHandlers.Businesses.Business.InventoryItems;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Receipts
{
    [ProtoContract]
    [Title(nameof(Strings.Receipts))]
    [Guide("The `Receipt Costs` screen provides a detailed breakdown of how costs are allocated across inventory items when processing receipts.")]
    [Guide("When you receive inventory items, the total cost of the receipt (including shipping, taxes, or other charges) needs to be distributed among the individual items. This screen shows you exactly how those costs have been allocated.")]
    [Guide("The cost allocation helps ensure accurate inventory valuation by properly assigning all receipt-related expenses to the specific items purchased.")]
    internal sealed class ReceiptCosts : TransactionCosts
    {
    }
}