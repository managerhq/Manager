using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.InventoryKits
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryKit), nameof(Strings.Edit))]
    [Guide("Create inventory kits that bundle multiple items together.")]
    [Guide("Kits automatically manage component inventory when sold or produced.")]
    [Fields(typeof(ManagerServer.Model.InventoryKit))]
    internal sealed class InventoryKitForm : NakedVueForm<ManagerServer.Model.InventoryKit>
    {
        protected override bool CanHaveImage() => true;
    }
}