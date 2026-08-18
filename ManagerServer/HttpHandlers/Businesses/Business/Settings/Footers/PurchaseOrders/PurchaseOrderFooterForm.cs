using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.PurchaseOrders
{
    [ProtoContract]
    [Title(nameof(Strings.Footer))]
    [Guide("Configure footer text that appears at the bottom of purchase orders.")]
    [Guide("Use footers to add terms, conditions, or additional information to purchase orders.")]
    [Fields(typeof(ManagerServer.Model.PurchaseOrderFooter))]
    internal sealed class PurchaseOrderFooterForm : NakedVueForm<ManagerServer.Model.PurchaseOrderFooter>
    {
    }
}