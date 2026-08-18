using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.InventoryLocations.DefaultInventoryLocation
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.DefaultInventoryLocation))]
    [Guide("Configure the default inventory location for all inventory items.")]
    [Guide("This location is used when no specific location is selected in transactions.")]
    [Fields(typeof(ManagerServer.Model.DefaultInventoryLocation))]
    internal sealed class DefaultInventoryLocationForm : NakedVueForm<ManagerServer.Model.DefaultInventoryLocation>
    {
        internal override bool IsEmpty(ManagerServer.Helpers.TabsExtensions.Item[] tabs)
        {
            return !ApplicationData.Businesses.Get(Business).Exists<ManagerServer.Model.DefaultInventoryLocation>();
        }
    }
}
