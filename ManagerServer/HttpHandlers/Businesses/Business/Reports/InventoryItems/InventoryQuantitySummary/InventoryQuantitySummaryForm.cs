using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryQuantitySummary
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryMovement))]
    [Guide("The `InventoryMovement` report tracks quantity changes for inventory items over a specified period.")]
    [Guide("View opening quantities, purchases, sales, adjustments, and closing balances for each item.")]
    [Guide("This report helps identify stock movements, detect discrepancies, and plan reorder points.")]
    [Guide("Configure date ranges to analyze seasonal trends or verify physical inventory counts.")]
    [Guide("Filter by specific items, locations, or categories to focus your inventory analysis.")]
    [Fields(typeof(ManagerServer.Model.InventoryQuantitySummary))]
    internal sealed class InventoryQuantitySummaryForm : NakedVueForm<ManagerServer.Model.InventoryQuantitySummary>
    {
    }
}
