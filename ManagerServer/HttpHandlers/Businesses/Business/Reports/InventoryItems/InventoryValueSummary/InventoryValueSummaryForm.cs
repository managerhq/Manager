using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryValueSummary
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryValueSummary))]
    [Guide("The `InventoryValueSummary` report shows the total cost value of your inventory holdings as of a specific date.")]
    [Guide("Each inventory item is valued using your chosen costing method (FIFO, average cost, or specific identification).")]
    [Guide("Use this report to verify inventory asset values for financial statements and insurance purposes.")]
    [Guide("Monitor inventory investment levels and identify slow-moving or high-value items requiring attention.")]
    [Guide("Configure the report date and filter by inventory location or item categories.")]
    [Fields(typeof(ManagerServer.Model.InventoryValueSummary))]
    internal sealed class InventoryValueSummaryForm : NakedVueForm<ManagerServer.Model.InventoryValueSummary>
    {
    }
}
