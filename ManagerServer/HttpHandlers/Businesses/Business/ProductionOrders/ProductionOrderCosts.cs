using ManagerServer.HttpHandlers.Businesses.Business.InventoryItems;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.ProductionOrders
{
    [ProtoContract]
    [Title(nameof(Strings.ProductionOrders))]
    [Guide("The **Production Order Costs** screen provides a detailed cost analysis for items manufactured through *production orders*.")]
    [Guide("This report helps you understand the total cost of producing finished goods by breaking down all input costs from your manufacturing process.")]
    [Header("Cost Analysis Features")]
    [Guide("For each *production order*, the system displays the cost of raw materials, components, and other inputs used in manufacturing.")]
    [Guide("The *cost per unit* is automatically calculated by dividing the total input costs by the quantity of finished goods produced.")]
    [Guide("All costs are presented in a clear, itemized format that shows exactly what went into producing your finished products.")]
    [Header("Using This Report")]
    [Guide("Use this cost information to analyze production efficiency and ensure your manufacturing processes are profitable.")]
    [Guide("The detailed breakdown helps you set appropriate selling prices based on actual production costs.")]
    [Guide("Review costs regularly to identify opportunities for cost reduction and improve your production margins.")]
    internal sealed class ProductionOrderCosts : TransactionCosts
    {
    }
}
