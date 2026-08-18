using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Query;
using HttpFramework;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.NonInventoryItems
{
    [ProtoContract]
    [Title(nameof(Strings.NonInventoryItem))]
    [Guide("Create items for services or products that you don't track in inventory.")]
    [Guide("Non-inventory items are useful for services, labor, or items you purchase and sell without stocking.")]
    [Fields(typeof(ManagerServer.Model.NonInventoryItem))]
    internal sealed class NonInventoryItemForm : NakedVueForm<ManagerServer.Model.NonInventoryItem>
    {
        protected override bool CanHaveImage() => true;
    }
}