using ManagerServer.HttpHandlers.Businesses.Business.InventoryItems;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.SalesInvoices
{
    [ProtoContract]
    [Title(nameof(Strings.SalesInvoices), nameof(Strings.CostOfGoodsSold))]
    [Guide("The **Sales Invoice Costs** screen displays the *cost of goods sold* for inventory items included on sales invoices.")]
    [Guide("This screen helps you track the actual cost associated with each inventory item sold, which is essential for calculating gross profit margins.")]
    [Guide("The system automatically calculates these costs based on your selected *inventory valuation method* (FIFO, LIFO, or average cost).")]
    [Guide("Use this information to analyze profitability by comparing the sales price against the allocated cost for each item.")]
    internal sealed class SalesInvoiceCosts : TransactionCosts
    {
    }
}
