using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.InventoryValueSummary;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryValueSummary
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryValueSummary))]
    [Guide("The Inventory Value Summary report shows monetary value movements.")]
    [Guide("It tracks opening values, purchases, cost of sales, and closing values.")]
    [LinkGuide("For more information see:", typeof(InventoryValueSummaryForm))]
    internal sealed class InventoryValueSummaryView : DefaultView<GetInventoryValueSummaryView>
    {
    }
}