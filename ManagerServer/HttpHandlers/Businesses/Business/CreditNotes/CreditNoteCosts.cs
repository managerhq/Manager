using ManagerServer.HttpHandlers.Businesses.Business.InventoryItems;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.CreditNotes
{
    [ProtoContract]
    [Title(nameof(Strings.CreditNote), nameof(Strings.CostOfGoodsSold))]
    [Guide("This screen displays the *cost of goods sold* calculation for inventory items being returned through a credit note.")]
    [Guide("When customers return inventory items, the system automatically calculates and reverses the associated costs from your *cost of goods sold* account.")]
    [Guide("The cost calculation uses your selected *inventory valuation method* to determine the appropriate cost to reverse.")]
    [Guide("The table below shows each inventory item being returned along with its calculated cost:")]
    [Columns]
    internal sealed class CreditNoteCosts : TransactionCosts
    {
    }
}
