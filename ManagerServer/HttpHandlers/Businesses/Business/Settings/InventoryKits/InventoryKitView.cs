using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query;
using ManagerServer.Helpers;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.InventoryKits
{
    [ProtoContract]
    [Guide("This page displays the complete details of an *inventory kit*, including its components and their quantities.")]
    [Guide("The *bill of materials* table shows each component item with its required quantity, item code, name, and description.")]
    [Guide("Inventory kits are useful for managing products that are assembled from multiple components or sold as bundles.")]
    [LinkGuide("To edit this inventory kit, see:", typeof(InventoryKitForm))]
    internal sealed class InventoryKitView : TransactionView<ManagerServer.Model.InventoryKit>
    {
        protected override bool CanHaveAttachments()
        {
            return false;
        }
    }
}