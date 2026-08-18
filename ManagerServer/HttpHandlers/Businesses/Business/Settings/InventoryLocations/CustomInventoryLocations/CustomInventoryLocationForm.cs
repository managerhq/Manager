using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.InventoryLocations.CustomInventoryLocations
{
    [ProtoContract]
    [Title(nameof(Strings.CustomInventoryLocation))]
    [Guide("Define custom inventory locations for tracking stock in different warehouses or areas.")]
    [Guide("Each location can track inventory quantities and movements separately.")]
    [Fields(typeof(ManagerServer.Model.CustomInventoryLocation))]
    internal sealed class CustomInventoryLocationForm : NakedVueForm<ManagerServer.Model.CustomInventoryLocation>
    {
    }
}
