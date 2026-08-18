using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Footers.SalesOrders
{
    [ProtoContract]
    [Title(nameof(Strings.Footer))]
    [Guide("Configure footer text that appears at the bottom of sales orders.")]
    [Guide("Use footers to add terms, conditions, or additional information to sales orders.")]
    [Fields(typeof(ManagerServer.Model.SalesOrderFooter))]
    internal sealed class SalesOrderFooterForm : NakedVueForm<ManagerServer.Model.SalesOrderFooter>
    {
    }
}
