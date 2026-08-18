using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Reports.InventoryPriceList
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryPriceList))]
    [Guide("The Inventory Price List form configures parameters for price list reports.")]
    [Guide("Select inventory items and price settings to generate custom price lists.")]
    [Fields(typeof(ManagerServer.Model.InventoryPriceList))]
    internal sealed class InventoryPriceListForm : NakedVueForm<ManagerServer.Model.InventoryPriceList>
    {        
    }
}
