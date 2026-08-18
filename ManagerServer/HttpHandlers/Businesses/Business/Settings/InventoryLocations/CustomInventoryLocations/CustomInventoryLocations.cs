using System;
using System.Linq;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.InventoryLocations.CustomInventoryLocations
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(InventoryItems))]
    [Title(nameof(Strings.CustomInventoryLocations))]
    [Guid("eb1d18d2-f2b6-4b03-be2f-73674c8e720b")]
    [Guide("*Inventory locations* allow you to track physical spaces where your inventory items are stored. This feature is found within the **Settings** tab.")]
    [Guide("This functionality is particularly useful for businesses that operate across multiple locations or have several storage facilities, warehouses, or retail outlets.")]
    [Guide("You can add new locations, edit existing location details, or deactivate locations that are no longer in use. Each location can be assigned a unique code for easy identification in transactions and reports.")]
    [SettingsItemScreenshot("fa-warehouse-alt", nameof(Strings.InventoryLocations))]
    internal sealed class CustomInventoryLocations : NakedObjectsWithAutomaticRows<ManagerServer.Model.CustomInventoryLocation>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            return !ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.CustomInventoryLocation>().Any();
        }

        [Guid("a7494ccd-b4b4-4177-8751-2a06feb831cc")]
        public string[] GetCode(ManagerServer.Model.CustomInventoryLocation[] rows)
        {
            return rows.Select(x => x.Code).ToArray();
        }

        [Default]
        [Guid("23d18632-e567-400d-8982-7abed4e7060f")]
        public string[] GetName(ManagerServer.Model.CustomInventoryLocation[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }
    }
}
