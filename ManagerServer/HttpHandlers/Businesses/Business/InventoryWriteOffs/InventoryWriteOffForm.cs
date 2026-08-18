using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.InventoryWriteOffs
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryWriteOff), nameof(Strings.Edit))]
    [Guide("The `Inventory Write-off` form allows you to record the removal of inventory items from your stock when they are lost, damaged, stolen, expired, or otherwise cannot be sold.")]
    [Guide("When you create an inventory write-off, the system automatically reduces the quantity on hand for the selected items and records the appropriate expense in your accounts.")]
    [Guide("Common reasons for inventory write-offs include product damage during storage, theft, expiration of perishable goods, obsolescence, or inventory count discrepancies.")]
    [Guide("The following fields are available on this form:")]
    [Fields(typeof(ManagerServer.Model.InventoryWriteOff))]
    internal sealed class InventoryWriteOffForm : NakedVueForm<ManagerServer.Model.InventoryWriteOff>
    {
        protected override bool CanHaveImage() => true;
    }
}