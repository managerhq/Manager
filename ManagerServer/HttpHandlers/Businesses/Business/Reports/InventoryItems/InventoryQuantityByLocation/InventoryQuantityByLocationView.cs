using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using ManagerServer.Api.Businesses.Business.Reports.InventoryQuantityByLocation;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryQuantityByLocation
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryQuantityByLocation))]
    [Guide("The Inventory Quantity by Location report shows inventory distribution.")]
    [Guide("It displays quantities of inventory items across different storage locations.")]
    [LinkGuide("For more information see:", typeof(InventoryQuantityByLocationForm))]
    internal sealed class InventoryQuantityByLocationView : DefaultView<GetInventoryQuantityByLocationView>
    {
    }
}