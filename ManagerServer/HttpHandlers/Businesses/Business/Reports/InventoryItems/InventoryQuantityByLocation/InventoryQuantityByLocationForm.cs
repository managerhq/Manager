using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryQuantityByLocation
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryQuantityByLocation))]
    [Guide("The Inventory Quantity by Location form configures location-based inventory reports.")]
    [Guide("Set date ranges to view inventory quantities across different storage locations.")]
    [Fields(typeof(ManagerServer.Model.InventoryQuantityByLocation))]
    internal sealed class InventoryQuantityByLocationForm : NakedVueForm<ManagerServer.Model.InventoryQuantityByLocation>
    {
    }
}
