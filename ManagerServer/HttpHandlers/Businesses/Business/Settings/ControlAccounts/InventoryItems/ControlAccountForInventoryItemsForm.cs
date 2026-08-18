using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using ManagerServer.Helpers;
using ManagerServer.Query;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ControlAccounts.InventoryItems
{
    [ProtoContract]
    [Title(nameof(Strings.InventoryOnHand), nameof(Strings.Edit))]
    [Guide("This form configures the control account for inventory on hand.")]
    [Guide("The control account tracks the value of inventory items on the balance sheet.")]
    [Fields(typeof(ControlAccountForInventoryItems))]
    internal sealed class ControlAccountForInventoryItemsForm : NakedVueForm<ControlAccountForInventoryItems>
    {
    }
}
