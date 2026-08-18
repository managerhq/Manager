using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.InventoryQuantitySummary;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryQuantitySummary
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryQuantitySummary))]
    [Guide("The Inventory Quantity Summary report shows inventory movements.")]
    [Guide("It tracks opening balances, purchases, sales, and closing quantities.")]
    [LinkGuide("For more information see:", typeof(InventoryQuantitySummaryForm))]
    internal sealed class InventoryQuantitySummaryView : DefaultView<GetInventoryQuantitySummaryView>
    {
    }
}