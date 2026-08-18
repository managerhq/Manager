using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.InventoryLocations
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.InventoryLocations))]
    [Guide("Inventory locations allow you to track *inventory quantities* separately for each physical location where you store goods, such as warehouses, retail stores, or different sections within a facility.")]
    [Guide("When inventory locations are enabled, every inventory transaction will require you to specify which location is affected, ensuring accurate tracking of stock levels at each site.")]
    [Guide("This feature is essential for businesses that operate from multiple locations and need to know exactly how much inventory is available at each site for fulfillment, transfers, and stock management purposes.")]
    [Guide("To get started, click the **New Inventory Location** button to add your first location. Common examples include main warehouse, retail store, online fulfillment center, or consignment locations.")]
    [Namespace(typeof(InventoryLocations))]
    internal sealed class InventoryLocations : NakedNamespaces
    {
    }
}